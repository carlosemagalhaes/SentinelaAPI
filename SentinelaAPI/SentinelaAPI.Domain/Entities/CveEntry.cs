namespace SentinelaAPI.Domain.Entities;

public class CveEntry
{
    public string CveId { get; private set; }
    public string Description { get; private set; }
    public double CvssScore { get; private set; }
    public string Severity { get; private set; }
    public string PackageName { get; private set; }
    public DateTime PublishedDate { get; private set; }

    private CveEntry() { }

    public static CveEntry Create(
        string cveId,
        string description,
        double cvssScore,
        string packageName,
        DateTime publishedDate)
    {
        return new CveEntry
        {
            CveId = cveId,
            Description = description,
            CvssScore = cvssScore,
            Severity = CalculateSeverity(cvssScore),
            PackageName = packageName,
            PublishedDate = publishedDate
        };
    }

    private static string CalculateSeverity(double score) => score switch
    {
        >= 9.0 => "Critical",
        >= 7.0 => "High",
        >= 4.0 => "Medium",
        >= 0.1 => "Low",
        _ => "None"
    };
}