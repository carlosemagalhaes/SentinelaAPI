using Microsoft.EntityFrameworkCore;
using SentinelaAPI.Domain.Entities;
using SentinelaAPI.Domain.Interfaces;
using SentinelaAPI.Infrastructure.Data;

namespace SentinelaAPI.Infrastructure.Repositories;

public class AnomalyAlertRepository : IAnomalyAlertRepository
{
    private readonly AppDbContext _context;

    public AnomalyAlertRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AnomalyAlert alert)
    {
        await _context.AnomalyAlerts.AddAsync(alert);
    }

    public async Task<IEnumerable<AnomalyAlert>> GetAllAsync()
    {
        return await _context.AnomalyAlerts
            .OrderByDescending(x => x.DetectedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AnomalyAlert>> GetUnresolvedAsync()
    {
        return await _context.AnomalyAlerts
            .Where(x => !x.IsResolved)
            .OrderByDescending(x => x.DetectedAt)
            .ToListAsync();
    }

    public async Task<AnomalyAlert?> GetByIdAsync(Guid id)
    {
        return await _context.AnomalyAlerts
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}