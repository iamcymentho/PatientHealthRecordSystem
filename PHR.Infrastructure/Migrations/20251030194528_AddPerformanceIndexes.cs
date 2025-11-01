using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccessRequests_PatientRecordId",
                table: "AccessRequests");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientRecords_CreatedByUserId_CreatedDateUtc_IsDeleted",
                table: "PatientRecords",
                columns: new[] { "CreatedByUserId", "CreatedDateUtc", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_PatientRecords_CreatedDateUtc",
                table: "PatientRecords",
                column: "CreatedDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRecords_Diagnosis",
                table: "PatientRecords",
                column: "Diagnosis");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRecords_PatientName",
                table: "PatientRecords",
                column: "PatientName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequests_PatientRecordId_Status",
                table: "AccessRequests",
                columns: new[] { "PatientRecordId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequests_RequestDateUtc",
                table: "AccessRequests",
                column: "RequestDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequests_RequestorUserId_Status",
                table: "AccessRequests",
                columns: new[] { "RequestorUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequests_Status",
                table: "AccessRequests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_PatientRecords_CreatedByUserId_CreatedDateUtc_IsDeleted",
                table: "PatientRecords");

            migrationBuilder.DropIndex(
                name: "IX_PatientRecords_CreatedDateUtc",
                table: "PatientRecords");

            migrationBuilder.DropIndex(
                name: "IX_PatientRecords_Diagnosis",
                table: "PatientRecords");

            migrationBuilder.DropIndex(
                name: "IX_PatientRecords_PatientName",
                table: "PatientRecords");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AccessRequests_PatientRecordId_Status",
                table: "AccessRequests");

            migrationBuilder.DropIndex(
                name: "IX_AccessRequests_RequestDateUtc",
                table: "AccessRequests");

            migrationBuilder.DropIndex(
                name: "IX_AccessRequests_RequestorUserId_Status",
                table: "AccessRequests");

            migrationBuilder.DropIndex(
                name: "IX_AccessRequests_Status",
                table: "AccessRequests");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequests_PatientRecordId",
                table: "AccessRequests",
                column: "PatientRecordId");
        }
    }
}
