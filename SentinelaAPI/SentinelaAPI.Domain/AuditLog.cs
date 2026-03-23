namespace SentinelaAPI.Domain.Entities;

using SentinelaAPI.Domain.Enums;

public class AuditLog
{
    public Guid Id { get; private set; }
    public AuditAction Action { get; private set; }
    public string Resource { get; private set; }
    public string? UserId { get; private set; }
    public string IpAddress { get; private set; }
    public int StatusCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsAnomaly { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        AuditAction action,
        string resource,
        string? userId,
        string ipAddress,
        int statusCode)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            Resource = resource,
            UserId = userId,
            IpAddress = ipAddress,
            StatusCode = statusCode,
            CreatedAt = DateTime.UtcNow,
            IsAnomaly = false
        };
    }

    public void MarkAsAnomaly()
    {
        IsAnomaly = true;
    }
}