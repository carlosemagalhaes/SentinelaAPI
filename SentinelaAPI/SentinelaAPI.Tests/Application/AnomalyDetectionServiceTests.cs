using FluentAssertions;
using Moq;
using SentinelaAPI.Application.Services;
using SentinelaAPI.Domain.Entities;
using SentinelaAPI.Domain.Enums;
using SentinelaAPI.Domain.Interfaces;

namespace SentinelaAPI.Tests.Application;

public class AnomalyDetectionServiceTests
{
    private readonly Mock<IAnomalyAlertRepository> _alertRepositoryMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepositoryMock;
    private readonly AnomalyDetectionService _service;

    public AnomalyDetectionServiceTests()
    {
        _alertRepositoryMock = new Mock<IAnomalyAlertRepository>();
        _auditLogRepositoryMock = new Mock<IAuditLogRepository>();
        _service = new AnomalyDetectionService(
            _alertRepositoryMock.Object,
            _auditLogRepositoryMock.Object);
    }

    [Fact]
    public async Task DetectAndAlertAsync_ShouldCreateBruteForceAlert_WhenThresholdExceeded()
    {
        // Arrange
        var ipAddress = "10.0.0.1";
        var now = DateTime.UtcNow;

        var logs = Enumerable.Range(0, 6).Select(_ =>
            AuditLog.Create(
                AuditAction.LoginFailed,
                "/api/auth/login",
                null,
                ipAddress,
                401)).ToList();

        _auditLogRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(logs);

        _alertRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<AnomalyAlert>()))
            .Returns(Task.CompletedTask);

        _alertRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.DetectAndAlertAsync(ipAddress, null, 401);

        // Assert
        _alertRepositoryMock.Verify(
            r => r.AddAsync(It.Is<AnomalyAlert>(a =>
                a.Type == AnomalyType.BruteForce &&
                a.IpAddress == ipAddress &&
                a.OccurrenceCount == 6)),
            Times.Once);
    }

    [Fact]
    public async Task DetectAndAlertAsync_ShouldNotCreateAlert_WhenBelowThreshold()
    {
        // Arrange
        var ipAddress = "10.0.0.1";

        var logs = Enumerable.Range(0, 3).Select(_ =>
            AuditLog.Create(
                AuditAction.LoginFailed,
                "/api/auth/login",
                null,
                ipAddress,
                401)).ToList();

        _auditLogRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(logs);

        // Act
        await _service.DetectAndAlertAsync(ipAddress, null, 401);

        // Assert
        _alertRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<AnomalyAlert>()),
            Times.Never);
    }

    [Fact]
    public async Task DetectAndAlertAsync_ShouldCreateScannerAlert_WhenThresholdExceeded()
    {
        // Arrange
        var ipAddress = "192.168.1.100";

        var logs = Enumerable.Range(0, 55).Select(i =>
            AuditLog.Create(
                AuditAction.Get,
                $"/api/resource/{i}",
                null,
                ipAddress,
                200)).ToList();

        _auditLogRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(logs);

        _alertRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<AnomalyAlert>()))
            .Returns(Task.CompletedTask);

        _alertRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.DetectAndAlertAsync(ipAddress, null, 200);

        // Assert
        _alertRepositoryMock.Verify(
            r => r.AddAsync(It.Is<AnomalyAlert>(a =>
                a.Type == AnomalyType.Scanner &&
                a.IpAddress == ipAddress)),
            Times.Once);
    }

    [Fact]
    public async Task DetectAndAlertAsync_ShouldCreateSuspiciousActivityAlert_WhenThresholdExceeded()
    {
        // Arrange
        var userId = "user-456";

        var logs = Enumerable.Range(0, 12).Select(_ =>
            AuditLog.Create(
                AuditAction.Get,
                "/api/admin",
                userId,
                "10.0.0.2",
                403)).ToList();

        _auditLogRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(logs);

        _alertRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<AnomalyAlert>()))
            .Returns(Task.CompletedTask);

        _alertRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.DetectAndAlertAsync("10.0.0.2", userId, 403);

        // Assert
        _alertRepositoryMock.Verify(
            r => r.AddAsync(It.Is<AnomalyAlert>(a =>
                a.Type == AnomalyType.SuspiciousActivity &&
                a.UserId == userId)),
            Times.Once);
    }

    [Fact]
    public async Task ResolveAlertAsync_ShouldResolveAlert_WhenExists()
    {
        // Arrange
        var alert = AnomalyAlert.Create(
            AnomalyType.BruteForce,
            "10.0.0.1",
            null,
            "Test alert",
            5);

        _alertRepositoryMock
            .Setup(r => r.GetByIdAsync(alert.Id))
            .ReturnsAsync(alert);

        _alertRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.ResolveAlertAsync(alert.Id);

        // Assert
        alert.IsResolved.Should().BeTrue();
        _alertRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ResolveAlertAsync_ShouldDoNothing_WhenAlertNotFound()
    {
        // Arrange
        _alertRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((AnomalyAlert?)null);

        // Act
        await _service.ResolveAlertAsync(Guid.NewGuid());

        // Assert
        _alertRepositoryMock.Verify(
            r => r.SaveChangesAsync(),
            Times.Never);
    }
}