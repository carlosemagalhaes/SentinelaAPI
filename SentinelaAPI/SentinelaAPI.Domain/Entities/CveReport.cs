namespace SentinelaAPI.Domain.Entities;

public class CveReport
{
    public Guid Id { get; private set; }
    public string PackageName { get; private set; }
    public string PackageVersion { get; private set; }
    public List<CveEntry> Vulnerabilities { get; private set; }
    public DateTime GeneratedAt { get; private set; }
    public int TotalVulnerabilities => Vulnerabilities.Count;
    public bool HasCritical => Vulnerabilities.Any(v => v.Severity == "Critical");
    public bool HasHigh => Vulnerabilities.Any(v => v.Severity == "High");

    private CveReport()
    {
        Vulnerabilities = new List<CveEntry>();
    }

    public static CveReport Create(string packageName, string packageVersion)
    {
        return new CveReport
        {
            Id = Guid.NewGuid(),
            PackageName = packageName,
            PackageVersion = packageVersion,
            Vulnerabilities = new List<CveEntry>(),
            GeneratedAt = DateTime.UtcNow
        };
    }

    public void AddVulnerability(CveEntry entry)
    {
        Vulnerabilities.Add(entry);
    }
}