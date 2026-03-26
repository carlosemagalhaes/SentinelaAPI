using SentinelaAPI.Domain.Entities;
using SentinelaAPI.Domain.Enums;

namespace SentinelaAPI.Application.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(AuditAction action, string resource, string? userId, string ipAddress, int statusCode);
    Task<IEnumerable<AuditLog>> GetAllAsync();
    Task<IEnumerable<AuditLog>> GetAnomaliesAsync();
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId);
}