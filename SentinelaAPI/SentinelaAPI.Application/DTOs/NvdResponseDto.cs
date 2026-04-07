using System.Text.Json.Serialization;

namespace SentinelaAPI.Application.DTOs;

public class NvdResponseDto
{
    [JsonPropertyName("vulnerabilities")]
    public List<NvdVulnerabilityDto> Vulnerabilities { get; set; } = new();
}

public class NvdVulnerabilityDto
{
    [JsonPropertyName("cve")]
    public NvdCveDto Cve { get; set; } = new();
}

public class NvdCveDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("published")]
    public DateTime Published { get; set; }

    [JsonPropertyName("descriptions")]
    public List<NvdDescriptionDto> Descriptions { get; set; } = new();

    [JsonPropertyName("metrics")]
    public NvdMetricsDto Metrics { get; set; } = new();
}

public class NvdDescriptionDto
{
    [JsonPropertyName("lang")]
    public string Lang { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public class NvdMetricsDto
{
    [JsonPropertyName("cvssMetricV31")]
    public List<NvdCvssMetricDto> CvssMetricV31 { get; set; } = new();

    [JsonPropertyName("cvssMetricV30")]
    public List<NvdCvssMetricDto> CvssMetricV30 { get; set; } = new();

    [JsonPropertyName("cvssMetricV2")]
    public List<NvdCvssMetricDto> CvssMetricV2 { get; set; } = new();
}

public class NvdCvssMetricDto
{
    [JsonPropertyName("cvssData")]
    public NvdCvssDataDto CvssData { get; set; } = new();
}

public class NvdCvssDataDto
{
    [JsonPropertyName("baseScore")]
    public double BaseScore { get; set; }
}