using SentinelaAPI.Domain.Enums;

namespace SentinelaAPI.Domain.Entities;

public class AnomalyAlert
{
    public Guid Id { get; private set; }
    public AnomalyType Type { get; private set; }
    public string IpAddress { get; private set; }
    public string? UserId { get; private set; }
    public string Description { get; private set; }
    public int OccurrenceCount { get; private set; }
    public DateTime DetectedAt { get; private set; }
    public bool IsResolved { get; private set; }

    private AnomalyAlert() { }

    public static AnomalyAlert Create(
        AnomalyType type,
        string ipAddress,
        string? userId,
        string description,
        int occurrenceCount)
    {
        return new AnomalyAlert
        {
            Id = Guid.NewGuid(),
            Type = type,
            IpAddress = ipAddress,
            UserId = userId,
            Description = description,
            OccurrenceCount = occurrenceCount,
            DetectedAt = DateTime.UtcNow,
            IsResolved = false
        };
    }

    public void Resolve()
    {
        IsResolved = true;
    }
}