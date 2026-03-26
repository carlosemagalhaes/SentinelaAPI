using SentinelaAPI.Domain.Entities;

namespace SentinelaAPI.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog);
    Task<IEnumerable<AuditLog>> GetAllAsync();
    Task<IEnumerable<AuditLog>> GetAnomaliesAsync();
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId);
    Task SaveChangesAsync();
}