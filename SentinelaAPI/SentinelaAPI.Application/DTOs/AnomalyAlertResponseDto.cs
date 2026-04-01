namespace SentinelaAPI.Application.DTOs;

public class AnomalyAlertResponseDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int OccurrenceCount { get; set; }
    public DateTime DetectedAt { get; set; }
    public bool IsResolved { get; set; }
}