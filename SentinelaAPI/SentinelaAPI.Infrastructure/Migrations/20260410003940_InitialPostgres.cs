using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelaAPI.Infrastructure.Migrations
{
    public partial class InitialPostgres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnomalyAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    UserId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    OccurrenceCount = table.Column<int>(type: "integer", nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnomalyAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Resource = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAnomaly = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnomalyAlerts_DetectedAt",
                table: "AnomalyAlerts",
                column: "DetectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AnomalyAlerts_IpAddress",
                table: "AnomalyAlerts",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_AnomalyAlerts_IsResolved",
                table: "AnomalyAlerts",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_IsAnomaly",
                table: "AuditLogs",
                column: "IsAnomaly");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AnomalyAlerts");
            migrationBuilder.DropTable(name: "AuditLogs");
        }
    }
}