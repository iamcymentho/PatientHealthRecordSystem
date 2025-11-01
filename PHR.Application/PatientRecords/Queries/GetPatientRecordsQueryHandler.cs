using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Data;
using PHR.Application.DTOs;
namespace PHR.Application.PatientRecords.Queries
{
	public class GetPatientRecordsQueryHandler : IRequestHandler<GetPatientRecordsQuery, IEnumerable<PatientRecordResponse>>
	{
		private readonly IApplicationDbContext _context;
		public GetPatientRecordsQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IEnumerable<PatientRecordResponse>> Handle(GetPatientRecordsQuery request, CancellationToken cancellationToken)
		{
			// Check if user has ViewPatientRecords permission
			var hasViewAllPermission = await _context.Users
				.Where(u => u.Id == request.UserId)
				.SelectMany(u => u.UserRoles)
				.SelectMany(ur => ur.Role.RolePermissions)
				.AnyAsync(rp => rp.Permission.Name == "ViewPatientRecords", cancellationToken);
			IQueryable<Domain.Entities.PatientRecord> query = _context.PatientRecords
				.Include(pr => pr.CreatedByUser);
			if (!hasViewAllPermission)
			{
				// If user doesn't have viewPatientRecords permission, only show their own records
				// and records they have approved access to (we'll handle access requests later)
				query = query.Where(pr => pr.CreatedByUserId == request.UserId);
			}
			var records = await query
				.OrderByDescending(pr => pr.CreatedDateUtc)
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