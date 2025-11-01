using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelForUniformResponses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessRequests_PatientRecords_PatientRecordId",
                table: "AccessRequests");

            migrationBuilder.AddColumn<string>(
                name: "Allergies",
                table: "PatientRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Imaging",
                table: "PatientRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LabResults",
                table: "PatientRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Medications",
                table: "PatientRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VitalSigns",
                table: "PatientRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", nullable: false),
                    EntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Details = table.Column<string>(type: "TEXT", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TimestampUtc",
                table: "AuditLogs",
                column: "TimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId_TimestampUtc",
                table: "AuditLogs",
                columns: new[] { "UserId", "TimestampUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_AccessRequests_PatientRecords_PatientRecordId",
                table: "AccessRequests",
                column: "PatientRecordId",
                principalTable: "PatientRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessRequests_PatientRecords_PatientRecordId",
                table: "AccessRequests");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Allergies",
                table: "PatientRecords");

            migrationBuilder.DropColumn(
                name: "Imaging",
                table: "PatientRecords");

            migrationBuilder.DropColumn(
                name: "LabResults",
                table: "PatientRecords");

            migrationBuilder.DropColumn(
                name: "Medications",
                table: "PatientRecords");

            migrationBuilder.DropColumn(
                name: "VitalSigns",
                table: "PatientRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessRequests_PatientRecords_PatientRecordId",
                table: "AccessRequests",
                column: "PatientRecordId",
                principalTable: "PatientRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
