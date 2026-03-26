using Microsoft.AspNetCore.Mvc;
using SentinelaAPI.Application.DTOs;
using SentinelaAPI.Application.Interfaces;
using SentinelaAPI.Domain.Enums;

namespace SentinelaAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _service;

    public AuditLogController(IAuditLogService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _service.GetAllAsync();
        var response = logs.Select(log => new AuditLogResponseDto
        {
            Id = log.Id,
            Action = log.Action.ToString(),
            Resource = log.Resource,
            UserId = log.UserId,
            IpAddress = log.IpAddress,
            StatusCode = log.StatusCode,
            CreatedAt = log.CreatedAt,
            IsAnomaly = log.IsAnomaly
        });
        return Ok(response);
    }

    [HttpGet("anomalies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAnomalies()
    {
        var logs = await _service.GetAnomaliesAsync();
        var response = logs.Select(log => new AuditLogResponseDto
        {
            Id = log.Id,
            Action = log.Action.ToString(),
            Resource = log.Resource,
            UserId = log.UserId,
            IpAddress = log.IpAddress,
            StatusCode = log.StatusCode,
            CreatedAt = log.CreatedAt,
            IsAnomaly = log.IsAnomaly
        });
        return Ok(response);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var logs = await _service.GetByUserIdAsync(userId);
        if (!logs.Any())
            return NotFound($"No logs found for user {userId}");

        var response = logs.Select(log => new AuditLogResponseDto
        {
            Id = log.Id,
            Action = log.Action.ToString(),
            Resource = log.Resource,
            UserId = log.UserId,
            IpAddress = log.IpAddress,
            StatusCode = log.StatusCode,
            CreatedAt = log.CreatedAt,
            IsAnomaly = log.IsAnomaly
        });
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAuditLogDto dto)
    {
        if (!Enum.TryParse<AuditAction>(dto.Action, true, out var action))
            return BadRequest($"Invalid action: {dto.Action}");

        await _service.LogAsync(
            action,
            dto.Resource,
            dto.UserId,
            dto.IpAddress,
            dto.StatusCode);

        return StatusCode(StatusCodes.Status201Created);
    }
}