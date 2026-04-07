namespace SentinelaAPI.Application.DTOs;

public class CveReportResponseDto
{
    public Guid Id { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string PackageVersion { get; set; } = string.Empty;
    public int TotalVulnerabilities { get; set; }
    public bool HasCritical { get; set; }
    public bool HasHigh { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<CveEntryDto> Vulnerabilities { get; set; } = new();
}

public class CveEntryDto
{
    public string CveId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double CvssScore { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
}