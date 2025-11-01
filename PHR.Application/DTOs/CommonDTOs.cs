using System;
using PHR.Domain.Entities.Enums;
namespace PHR.Application.DTOs
{
	// Request DTOs
	public record CreatePatientRecordRequest(
		string PatientName,
		DateTime DateOfBirth,
		string Diagnosis,
		string TreatmentPlan,
		string MedicalHistory);
	public record UpdatePatientRecordRequest(
		string PatientName,
		DateTime DateOfBirth,
		string Diagnosis,
		string TreatmentPlan,
		string MedicalHistory);
	public record CreateAccessRequestDto(
		Guid PatientRecordId,
		string Reason);
	public record ApproveAccessRequestDto(
		DateTime ApprovedStartUtc,
		DateTime ApprovedEndUtc);
	public record DeclineAccessRequestDto(
		string DeclineReason);
	// Audit DTOs
	public record AuditLogResponse(
		Guid Id,
		Guid UserId,
		string UserName,
		string Action,
		string EntityType,
		Guid? EntityId,
		string Details,
		DateTime TimestampUtc,
		string IpAddress);
	public record GetAuditLogsRequest(
		Guid? UserId = null,
		string? EntityType = null,
		Guid? EntityId = null,
		DateTime? FromDate = null,
		DateTime? ToDate = null,
		int PageNumber = 1,
		int PageSize = 50);
	// Response DTOs
	public record PatientRecordResponse(
		Guid Id,
		string PatientName,
		DateTime DateOfBirth,
		string Diagnosis,
		string TreatmentPlan,
		string MedicalHistory,
		string Medications,
		string Allergies,
		string VitalSigns,
		string LabResults,
		string Imaging,
		Guid CreatedByUserId,
		DateTime CreatedDateUtc,
		Guid? LastModifiedByUserId,
		DateTime? LastModifiedDateUtc,
		string CreatedByUserName);
	public record AccessRequestResponse(
		Guid Id,
		Guid PatientRecordId,
		string PatientRecordPatientName,
		Guid RequestorUserId,
		string RequestorUserName,
		string Reason,
		DateTime RequestDateUtc,
		AccessRequestStatus Status,
		Guid? ApprovedByUserId,
		string? ApprovedByUserName,
		DateTime? ApprovedStartUtc,
		DateTime? ApprovedEndUtc,
		DateTime? DecisionDateUtc,
		string? DeclineReason);
	public record UserResponse(
		Guid Id,
		string FullName,
		string Email,
		Gender Gender,
		string? PhoneNumber,
		bool IsActive,
		DateTime CreatedAtUtc,
		IEnumerable<string> Roles);
	public record RoleResponse(
		Guid Id,
		string Name,
		IEnumerable<string> Permissions);
}