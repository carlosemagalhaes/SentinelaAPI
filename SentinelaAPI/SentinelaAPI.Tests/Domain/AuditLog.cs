using FluentAssertions;
using SentinelaAPI.Domain.Entities;
using SentinelaAPI.Domain.Enums;

namespace SentinelaAPI.Tests.Domain;

public class AuditLogTests
{
    [Fact]
    public void Create_ShouldCreateAuditLog_WithCorrectProperties()
    {
        // Arrange
        var action = AuditAction.Login;
        var resource = "/api/auth/login";
        var userId = "user-123";
        var ipAddress = "127.0.0.1";
        var statusCode = 200;

        // Act
        var auditLog = AuditLog.Create(action, resource, userId, ipAddress, statusCode);

        // Assert
        auditLog.Should().NotBeNull();
        auditLog.Action.Should().Be(action);
        auditLog.Resource.Should().Be(resource);
        auditLog.UserId.Should().Be(userId);
        auditLog.IpAddress.Should().Be(ipAddress);
        auditLog.StatusCode.Should().Be(statusCode);
        auditLog.IsAnomaly.Should().BeFalse();
        auditLog.Id.Should().NotBeEmpty();
        auditLog.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_ShouldCreateAuditLog_WithNullUserId()
    {
        // Arrange & Act
        var auditLog = AuditLog.Create(
            AuditAction.Get,
            "/api/auditlog",
            null,
            "192.168.1.1",
            200);

        // Assert
        auditLog.UserId.Should().BeNull();
        auditLog.IsAnomaly.Should().BeFalse();
    }

    [Fact]
    public void MarkAsAnomaly_ShouldSetIsAnomaly_ToTrue()
    {
        // Arrange
        var auditLog = AuditLog.Create(
            AuditAction.LoginFailed,
            "/api/auth/login",
            null,
            "10.0.0.1",
            401);

        // Act
        auditLog.MarkAsAnomaly();

        // Assert
        auditLog.IsAnomaly.Should().BeTrue();
    }

    [Theory]
    [InlineData(AuditAction.Get, "/api/test", 200)]
    [InlineData(AuditAction.Post, "/api/users", 201)]
    [InlineData(AuditAction.Delete, "/api/users/1", 204)]
    [InlineData(AuditAction.LoginFailed, "/api/auth", 401)]
    public void Create_ShouldWork_ForAllActions(
        AuditAction action, string resource, int statusCode)
    {
        // Arrange & Act
        var auditLog = AuditLog.Create(action, resource, null, "127.0.0.1", statusCode);

        // Assert
        auditLog.Action.Should().Be(action);
        auditLog.StatusCode.Should().Be(statusCode);
    }
}