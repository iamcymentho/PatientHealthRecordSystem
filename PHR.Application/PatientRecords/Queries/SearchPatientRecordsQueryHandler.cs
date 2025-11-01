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
	public class SearchPatientRecordsQueryHandler : IRequestHandler<SearchPatientRecordsQuery, IEnumerable<PatientRecordResponse>>
	{
		private readonly IApplicationDbContext _context;
		public SearchPatientRecordsQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IEnumerable<PatientRecordResponse>> Handle(SearchPatientRecordsQuery request, CancellationToken cancellationToken)
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
			// Apply search across multiple fields
			var searchTerm = request.SearchTerm.ToLower();
			query = query.Where(pr => 
				pr.PatientName.ToLower().Contains(searchTerm) ||
				pr.Diagnosis.ToLower().Contains(searchTerm) ||
				pr.TreatmentPlan.ToLower().Contains(searchTerm) ||
				pr.Medications.ToLower().Contains(searchTerm) ||
				pr.Allergies.ToLower().Contains(searchTerm) ||
				pr.Id.ToString().Contains(searchTerm)
			);
			var records = await query
				.OrderByDescending(pr => pr.CreatedDateUtc)
				.Take(100) // Limit search results
				.ToListAsync(cancellationToken);
			return records.Select(pr => new PatientRecordResponse(
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
		}
	}
}