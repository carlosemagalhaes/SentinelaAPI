using SentinelaAPI.Domain.Entities;

namespace SentinelaAPI.Domain.Interfaces;

public interface IAnomalyAlertRepository
{
    Task AddAsync(AnomalyAlert alert);
    Task<IEnumerable<AnomalyAlert>> GetAllAsync();
    Task<IEnumerable<AnomalyAlert>> GetUnresolvedAsync();
    Task<AnomalyAlert?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}