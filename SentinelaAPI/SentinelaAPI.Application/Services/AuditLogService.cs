using SentinelaAPI.Application.DTOs;
using SentinelaAPI.Application.Interfaces;
using SentinelaAPI.Domain.Entities;
using SentinelaAPI.Domain.Enums;
using SentinelaAPI.Domain.Interfaces;

namespace SentinelaAPI.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task LogAsync(
        AuditAction action,
        string resource,
        string? userId,
        string ipAddress,
        int statusCode)
    {
        var auditLog = AuditLog.Create(action, resource, userId, ipAddress, statusCode);
        await _repository.AddAsync(auditLog);
        await _repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAnomaliesAsync()
    {
        return await _repository.GetAnomaliesAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }
}