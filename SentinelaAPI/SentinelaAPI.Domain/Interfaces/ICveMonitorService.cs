using SentinelaAPI.Domain.Entities;

namespace SentinelaAPI.Domain.Interfaces;

public interface ICveMonitorService
{
    Task<IEnumerable<CveReport>> ScanProjectAsync(string csprojPath);
    Task<CveReport> ScanPackageAsync(string packageName, string version);
}