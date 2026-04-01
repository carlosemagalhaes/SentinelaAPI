using SentinelaAPI.Domain.Entities;

namespace SentinelaAPI.Application.Interfaces;

public interface IAnomalyDetectionService
{
    Task DetectAndAlertAsync(string ipAddress, string? userId, int statusCode);
    Task<IEnumerable<AnomalyAlert>> GetAllAlertsAsync();
    Task<IEnumerable<AnomalyAlert>> GetUnresolvedAlertsAsync();
    Task ResolveAlertAsync(Guid id);
}