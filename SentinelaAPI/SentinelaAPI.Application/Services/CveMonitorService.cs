using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using SentinelaAPI.Application.DTOs;
using SentinelaAPI.Domain.Entities;
using SentinelaAPI.Domain.Interfaces;

namespace SentinelaAPI.Application.Services;

public class CveMonitorService : ICveMonitorService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CveMonitorService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CveMonitorService(HttpClient httpClient, ILogger<CveMonitorService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<CveReport>> ScanProjectAsync(string csprojPath)
    {
        if (!File.Exists(csprojPath))
            throw new FileNotFoundException($"Project file not found: {csprojPath}");

        var packages = ExtractPackagesFromCsproj(csprojPath);
        var reports = new List<CveReport>();

        foreach (var package in packages)
        {
            try
            {
                await Task.Delay(500);
                var report = await ScanPackageAsync(package.Key, package.Value);
                reports.Add(report);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to scan package {Package}", package.Key);
            }
        }

        return reports;
    }

    public async Task<CveReport> ScanPackageAsync(string packageName, string version)
    {
        var report = CveReport.Create(packageName, version);

        try
        {
            var url = $"https://services.nvd.nist.gov/rest/json/cves/2.0?keywordSearch={Uri.EscapeDataString(packageName)}&resultsPerPage=20";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("NVD API returned {StatusCode} for {Package}",
                    response.StatusCode, packageName);
                return report;
            }

            var json = await response.Content.ReadAsStringAsync();
            var nvdResponse = JsonSerializer.Deserialize<NvdResponseDto>(json, JsonOptions);

            if (nvdResponse?.Vulnerabilities == null)
                return report;

            foreach (var vuln in nvdResponse.Vulnerabilities)
            {
                var cve = vuln.Cve;
                var description = cve.Descriptions
                    .FirstOrDefault(d => d.Lang == "en")?.Value ?? "No description available";

                var score = GetCvssScore(cve.Metrics);

                var entry = CveEntry.Create(
                    cve.Id,
                    description,
                    score,
                    packageName,
                    cve.Published);

                report.AddVulnerability(entry);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning package {Package}", packageName);
        }

        return report;
    }

    private static double GetCvssScore(NvdMetricsDto metrics)
    {
        if (metrics.CvssMetricV31.Any())
            return metrics.CvssMetricV31[0].CvssData.BaseScore;

        if (metrics.CvssMetricV30.Any())
            return metrics.CvssMetricV30[0].CvssData.BaseScore;

        if (metrics.CvssMetricV2.Any())
            return metrics.CvssMetricV2[0].CvssData.BaseScore;

        return 0.0;
    }

    private static Dictionary<string, string> ExtractPackagesFromCsproj(string csprojPath)
    {
        var packages = new Dictionary<string, string>();

        try
        {
            var xml = XDocument.Load(csprojPath);
            var packageRefs = xml.Descendants("PackageReference");

            foreach (var pkg in packageRefs)
            {
                var name = pkg.Attribute("Include")?.Value;
                var version = pkg.Attribute("Version")?.Value ?? "unknown";

                if (!string.IsNullOrEmpty(name))
                    packages[name] = version;
            }
        }
        catch (Exception)
        {
            // return empty if file can't be parsed
        }

        return packages;
    }
}