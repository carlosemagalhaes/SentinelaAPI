using Microsoft.AspNetCore.Mvc;
using SentinelaAPI.Application.DTOs;
using SentinelaAPI.Application.Interfaces;

namespace SentinelaAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnomalyAlertController : ControllerBase
{
    private readonly IAnomalyDetectionService _service;

    public AnomalyAlertController(IAnomalyDetectionService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var alerts = await _service.GetAllAlertsAsync();
        var response = alerts.Select(a => new AnomalyAlertResponseDto
        {
            Id = a.Id,
            Type = a.Type.ToString(),
            IpAddress = a.IpAddress,
            UserId = a.UserId,
            Description = a.Description,
            OccurrenceCount = a.OccurrenceCount,
            DetectedAt = a.DetectedAt,
            IsResolved = a.IsResolved
        });
        return Ok(response);
    }

    [HttpGet("unresolved")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnresolved()
    {
        var alerts = await _service.GetUnresolvedAlertsAsync();
        var response = alerts.Select(a => new AnomalyAlertResponseDto
        {
            Id = a.Id,
            Type = a.Type.ToString(),
            IpAddress = a.IpAddress,
            UserId = a.UserId,
            Description = a.Description,
            OccurrenceCount = a.OccurrenceCount,
            DetectedAt = a.DetectedAt,
            IsResolved = a.IsResolved
        });
        return Ok(response);
    }

    [HttpPatch("{id}/resolve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resolve(Guid id)
    {
        await _service.ResolveAlertAsync(id);
        return NoContent();
    }
}