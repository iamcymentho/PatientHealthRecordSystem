using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Data;
using PHR.Application.DTOs;
using PHR.Domain.Constants;
using PHR.Domain.Entities.Enums;
namespace PHR.Application.PatientRecords.Queries
{
	public class GetPatientRecordsWithFiltersQueryHandler : IRequestHandler<GetPatientRecordsWithFiltersQuery, PatientRecordsPagedResponse>
	{
		private readonly IApplicationDbContext _context;
		public GetPatientRecordsWithFiltersQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<PatientRecordsPagedResponse> Handle(GetPatientRecordsWithFiltersQuery request, CancellationToken cancellationToken)
		{
			// Check if user has ViewPatientRecords permission
			var hasViewAllPermission = await _context.Users
				.Where(u => u.Id == request.UserId)
				.SelectMany(u => u.UserRoles)
				.SelectMany(ur => ur.Role.RolePermissions)
				.AnyAsync(rp => rp.Permission.Name == Permissions.ViewPatientRecords, cancellationToken);
			IQueryable<Domain.Entities.PatientRecord> query = _context.PatientRecords
				.Include(pr => pr.CreatedByUser);
			// Authorization: Apply user-specific filtering
			if (!hasViewAllPermission)
			{
				// If user doesn't have ViewPatientRecords permission, only show their own records
				// and records they have active approved access to
				var currentDateTime = DateTime.UtcNow;
				query = query.Where(pr => 
					pr.CreatedByUserId == request.UserId ||
					pr.AccessRequests.Any(ar => 
						ar.RequestorUserId == request.UserId &&
						ar.Status == AccessRequestStatus.Approved &&
						ar.ApprovedStartUtc <= currentDateTime &&
						ar.ApprovedEndUtc >= currentDateTime
					)
				);
			}
			// Apply filters
			if (!string.IsNullOrWhiteSpace(request.PatientName))
			{
				query = query.Where(pr => pr.PatientName.Contains(request.PatientName));
			}
			if (request.DateFromUtc.HasValue)
			{
				query = query.Where(pr => pr.CreatedDateUtc >= request.DateFromUtc.Value);
			}
			if (request.DateToUtc.HasValue)
			{
				query = query.Where(pr => pr.CreatedDateUtc <= request.DateToUtc.Value);
			}
			if (!string.IsNullOrWhiteSpace(request.Diagnosis))
			{
				query = query.Where(pr => pr.Diagnosis.Contains(request.Diagnosis));
			}
			// Get total count before pagination
			var totalCount = await query.CountAsync(cancellationToken);
			// Apply pagination
			var records = await query
				.OrderByDescending(pr => pr.CreatedDateUtc)
				.Skip((request.PageNumber - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync(cancellationToken);
			var recordResponses = records.Select(pr => new PatientRecordResponse(
				pr.Id,
				pr.PatientName,
				pr.DateOfBirth,
				pr.Diagnosis,
				pr.TreatmentPlan,
				pr.MedicalHistory,
				pr.Medications,
				pr.Allergies,
				pr.VitalSigns,
				pr.LabResults,
				pr.Imaging,
				pr.CreatedByUserId,
				pr.CreatedDateUtc,
				pr.LastModifiedByUserId,
				pr.LastModifiedDateUtc,
				pr.CreatedByUser?.FullName ?? "Unknown User"
			));
			var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
			return new PatientRecordsPagedResponse(
				recordResponses,
				totalCount,
				request.PageNumber,
				request.PageSize,
				totalPages
			);
		}
	}
}