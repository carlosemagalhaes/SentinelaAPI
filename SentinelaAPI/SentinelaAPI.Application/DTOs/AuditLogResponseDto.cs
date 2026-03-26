namespace SentinelaAPI.Application.DTOs;

public class AuditLogResponseDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAnomaly { get; set; }
}