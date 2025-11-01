using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Data;
using PHR.Application.Abstractions.Services;
using PHR.Application.DTOs;
namespace PHR.Application.PatientRecords.Queries
{
	public class GetPatientRecordByIdQueryHandler : IRequestHandler<GetPatientRecordByIdQuery, PatientRecordResponse?>
	{
		private readonly IApplicationDbContext _context;
		private readonly IAuditService _auditService;
		public GetPatientRecordByIdQueryHandler(IApplicationDbContext context, IAuditService auditService)
		{
			_context = context;
			_auditService = auditService;
		}
		public async Task<PatientRecordResponse?> Handle(GetPatientRecordByIdQuery request, CancellationToken cancellationToken)
		{
			// Check if user has ViewPatientRecords permission
			var hasViewAllPermission = await _context.Users
				.Where(u => u.Id == request.UserId)
				.SelectMany(u => u.UserRoles)
				.SelectMany(ur => ur.Role.RolePermissions)
				.AnyAsync(rp => rp.Permission.Name == "ViewPatientRecords", cancellationToken);
			var patientRecord = await _context.PatientRecords
				.Include(pr => pr.CreatedByUser)
				.Include(pr => pr.LastModifiedByUser)
				.FirstOrDefaultAsync(pr => pr.Id == request.Id, cancellationToken);
			if (patientRecord == null)
			{
				return null;
			}
			// Check if user can access this record:
			// 1. They created it, OR
			// 2. They have ViewPatientRecords permission, OR  
			// 3. They have approved access request (TODO: implement)
			bool canView = patientRecord.CreatedByUserId == request.UserId || hasViewAllPermission;
			if (!canView)
			{
				// Check for approved access requests
				var hasApprovedAccess = await _context.AccessRequests
					.AnyAsync(ar => ar.PatientRecordId == request.Id &&
					               ar.RequestorUserId == request.UserId &&
					               ar.Status == Domain.Entities.Enums.AccessRequestStatus.Approved &&
					               ar.ApprovedStartUtc <= DateTime.UtcNow &&
					               ar.ApprovedEndUtc >= DateTime.UtcNow, cancellationToken);
				if (!hasApprovedAccess)
				{
					return null; // User cannot access this record
				}
			}
			if (!canView)
			{
				throw new UnauthorizedAccessException("You don't have permission to view this patient record.");
			}
			// Log the access for audit trail
			await _auditService.LogPatientRecordAccessAsync(
				request.UserId,
				patientRecord.Id,
				patientRecord.PatientName);
			return new PatientRecordResponse(
				patientRecord.Id,
				patientRecord.PatientName,
				patientRecord.DateOfBirth,
				patientRecord.Diagnosis,
				patientRecord.TreatmentPlan,
				patientRecord.MedicalHistory,
				patientRecord.Medications,
				patientRecord.Allergies,
				patientRecord.VitalSigns,
				patientRecord.LabResults,
				patientRecord.Imaging,
				patientRecord.CreatedByUserId,
				patientRecord.CreatedDateUtc,
				patientRecord.LastModifiedByUserId,
				patientRecord.LastModifiedDateUtc,
				patientRecord.CreatedByUser?.FullName ?? "Unknown User"
			);
		}
	}
}