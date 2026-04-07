using Microsoft.AspNetCore.Mvc;
using SentinelaAPI.Application.DTOs;
using SentinelaAPI.Domain.Interfaces;

namespace SentinelaAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CveMonitorController : ControllerBase
{
    private readonly ICveMonitorService _service;

    public CveMonitorController(ICveMonitorService service)
    {
        _service = service;
    }

    [HttpGet("scan-package")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScanPackage(
        [FromQuery] string packageName,
        [FromQuery] string version = "latest")
    {
        if (string.IsNullOrWhiteSpace(packageName))
            return BadRequest("Package name is required");

        var report = await _service.ScanPackageAsync(packageName, version);

        var response = new CveReportResponseDto
        {
            Id = report.Id,
            PackageName = report.PackageName,
            PackageVersion = report.PackageVersion,
            TotalVulnerabilities = report.TotalVulnerabilities,
            HasCritical = report.HasCritical,
            HasHigh = report.HasHigh,
            GeneratedAt = report.GeneratedAt,
            Vulnerabilities = report.Vulnerabilities.Select(v => new CveEntryDto
            {
                CveId = v.CveId,
                Description = v.Description,
                CvssScore = v.CvssScore,
                Severity = v.Severity,
                PackageName = v.PackageName,
                PublishedDate = v.PublishedDate
            }).ToList()
        };

        return Ok(response);
    }

    [HttpGet("scan-project")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ScanProject([FromQuery] string csprojPath)
    {
        if (string.IsNullOrWhiteSpace(csprojPath))
            return BadRequest("Project path is required");

        if (!System.IO.File.Exists(csprojPath))
            return NotFound($"File not found: {csprojPath}");

        var reports = await _service.ScanProjectAsync(csprojPath);

        var response = reports.Select(report => new CveReportResponseDto
        {
            Id = report.Id,
            PackageName = report.PackageName,
            PackageVersion = report.PackageVersion,
            TotalVulnerabilities = report.TotalVulnerabilities,
            HasCritical = report.HasCritical,
            HasHigh = report.HasHigh,
            GeneratedAt = report.GeneratedAt,
            Vulnerabilities = report.Vulnerabilities.Select(v => new CveEntryDto
            {
                CveId = v.CveId,
                Description = v.Description,
                CvssScore = v.CvssScore,
                Severity = v.Severity,
                PackageName = v.PackageName,
                PublishedDate = v.PublishedDate
            }).ToList()
        });

        return Ok(response);
    }

    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectSummary([FromQuery] string csprojPath)
    {
        if (string.IsNullOrWhiteSpace(csprojPath))
            return BadRequest("Project path is required");

        if (!System.IO.File.Exists(csprojPath))
            return NotFound($"File not found: {csprojPath}");

        var reports = await _service.ScanProjectAsync(csprojPath);
        var reportList = reports.ToList();

        var summary = new
        {
            ScannedAt = DateTime.UtcNow,
            TotalPackages = reportList.Count,
            TotalVulnerabilities = reportList.Sum(r => r.TotalVulnerabilities),
            CriticalPackages = reportList.Count(r => r.HasCritical),
            HighPackages = reportList.Count(r => r.HasHigh),
            Packages = reportList.Select(r => new
            {
                r.PackageName,
                r.PackageVersion,
                r.TotalVulnerabilities,
                r.HasCritical,
                r.HasHigh
            })
        };

        return Ok(summary);
    }
}