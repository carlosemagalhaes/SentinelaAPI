using SentinelaAPI.Application.Interfaces;
using SentinelaAPI.Domain.Entities;
using SentinelaAPI.Domain.Enums;
using SentinelaAPI.Domain.Interfaces;

namespace SentinelaAPI.Application.Services;

public class AnomalyDetectionService : IAnomalyDetectionService
{
    private readonly IAnomalyAlertRepository _alertRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public AnomalyDetectionService(
        IAnomalyAlertRepository alertRepository,
        IAuditLogRepository auditLogRepository)
    {
        _alertRepository = alertRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task DetectAndAlertAsync(string ipAddress, string? userId, int statusCode)
    {
        var now = DateTime.UtcNow;
        var logs = await _auditLogRepository.GetAllAsync();

        await CheckBruteForce(logs, ipAddress, now);
        await CheckScanner(logs, ipAddress, now);

        if (userId != null)
            await CheckSuspiciousActivity(logs, userId, now);
    }

    private async Task CheckBruteForce(
        IEnumerable<AuditLog> logs, string ipAddress, DateTime now)
    {
        var window = now.AddMinutes(-5);
        var count = logs.Count(l =>
            l.IpAddress == ipAddress &&
            l.Action == AuditAction.LoginFailed &&
            l.CreatedAt >= window);

        if (count >= 5)
        {
            var alert = AnomalyAlert.Create(
                AnomalyType.BruteForce,
                ipAddress,
                null,
                $"Brute force detected: {count} failed login attempts in 5 minutes from IP {ipAddress}",
                count);

            await _alertRepository.AddAsync(alert);
            await _alertRepository.SaveChangesAsync();
        }
    }

    private async Task CheckScanner(
        IEnumerable<AuditLog> logs, string ipAddress, DateTime now)
    {
        var window = now.AddMinutes(-1);
        var count = logs.Count(l =>
            l.IpAddress == ipAddress &&
            l.CreatedAt >= window);

        if (count >= 50)
        {
            var alert = AnomalyAlert.Create(
                AnomalyType.Scanner,
                ipAddress,
                null,
                $"Scanner detected: {count} requests in 1 minute from IP {ipAddress}",
                count);

            await _alertRepository.AddAsync(alert);
            await _alertRepository.SaveChangesAsync();
        }
    }

    private async Task CheckSuspiciousActivity(
        IEnumerable<AuditLog> logs, string userId, DateTime now)
    {
        var window = now.AddMinutes(-10);
        var count = logs.Count(l =>
            l.UserId == userId &&
            (l.StatusCode == 401 || l.StatusCode == 403) &&
            l.CreatedAt >= window);

        if (count >= 10)
        {
            var alert = AnomalyAlert.Create(
                AnomalyType.SuspiciousActivity,
                string.Empty,
                userId,
                $"Suspicious activity: {count} unauthorized attempts by user {userId} in 10 minutes",
                count);

            await _alertRepository.AddAsync(alert);
            await _alertRepository.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<AnomalyAlert>> GetAllAlertsAsync()
    {
        return await _alertRepository.GetAllAsync();
    }

    public async Task<IEnumerable<AnomalyAlert>> GetUnresolvedAlertsAsync()
    {
        return await _alertRepository.GetUnresolvedAsync();
    }

    public async Task ResolveAlertAsync(Guid id)
    {
        var alert = await _alertRepository.GetByIdAsync(id);
        if (alert is null) return;

        alert.Resolve();
        await _alertRepository.SaveChangesAsync();
    }
}