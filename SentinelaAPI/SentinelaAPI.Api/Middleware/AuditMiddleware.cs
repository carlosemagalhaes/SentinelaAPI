using SentinelaAPI.Application.Interfaces;
using SentinelaAPI.Domain.Enums;
using System.Security.Claims;

namespace SentinelaAPI.Api.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;

    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuditLogService auditLogService,
        IAnomalyDetectionService anomalyDetectionService)
    {
        var path = context.Request.Path.Value ?? "/";
        var skipPaths = new[] { "/swagger", "/favicon.ico", "/health" };

        if (skipPaths.Any(s => path.StartsWith(s, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        await _next(context);

        try
        {
            var action = GetAuditAction(context.Request.Method);
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ipAddress = GetIpAddress(context);
            var statusCode = context.Response.StatusCode;

            await auditLogService.LogAsync(action, path, userId, ipAddress, statusCode);
            await anomalyDetectionService.DetectAndAlertAsync(ipAddress, userId, statusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process audit log for {Path}", path);
        }
    }

    private static AuditAction GetAuditAction(string method) => method.ToUpper() switch
    {
        "GET" => AuditAction.Get,
        "POST" => AuditAction.Post,
        "PUT" => AuditAction.Put,
        "DELETE" => AuditAction.Delete,
        "PATCH" => AuditAction.Patch,
        _ => AuditAction.Get
    };

    private static string GetIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
            return forwardedFor.Split(',')[0].Trim();

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}