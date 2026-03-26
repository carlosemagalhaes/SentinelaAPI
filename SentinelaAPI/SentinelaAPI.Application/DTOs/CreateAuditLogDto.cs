namespace SentinelaAPI.Application.DTOs;

public class CreateAuditLogDto
{
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}