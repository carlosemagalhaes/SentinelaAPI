using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelaAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnomalyAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnomalyAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OccurrenceCount = table.Column<int>(type: "int", nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnomalyAlerts", x => x.Id);
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnomalyAlerts");
        }
    }
}
