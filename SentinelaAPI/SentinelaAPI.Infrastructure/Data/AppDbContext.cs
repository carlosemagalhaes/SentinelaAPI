using Microsoft.EntityFrameworkCore;
using SentinelaAPI.Domain.Entities;
using SentinelaAPI.Domain.Enums;

namespace SentinelaAPI.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Action)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Resource)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsAnomaly);
        });
    }
}