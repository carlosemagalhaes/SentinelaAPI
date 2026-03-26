using Microsoft.EntityFrameworkCore;
using SentinelaAPI.Domain.Entities;
using SentinelaAPI.Domain.Interfaces;
using SentinelaAPI.Infrastructure.Data;

namespace SentinelaAPI.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAnomaliesAsync()
    {
        return await _context.AuditLogs
            .Where(x => x.IsAnomaly)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId)
    {
        return await _context.AuditLogs
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}