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
namespace PHR.Application.AccessRequests.Queries
{
	public class GetAccessRequestsWithFiltersQueryHandler : IRequestHandler<GetAccessRequestsWithFiltersQuery, AccessRequestsPagedResponse>
	{
		private readonly IApplicationDbContext _context;
		public GetAccessRequestsWithFiltersQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<AccessRequestsPagedResponse> Handle(GetAccessRequestsWithFiltersQuery request, CancellationToken cancellationToken)
		{
			// Check if user has approval permissions
			var hasApprovalPermission = await _context.Users
				.Where(u => u.Id == request.UserId)
				.SelectMany(u => u.UserRoles)
				.SelectMany(ur => ur.Role.RolePermissions)
				.AnyAsync(rp => rp.Permission.Name == Permissions.ApproveAccessRequests, cancellationToken);
			IQueryable<Domain.Entities.AccessRequest> query = _context.AccessRequests
				.Include(ar => ar.RequestorUser)
				.Include(ar => ar.PatientRecord)
				.Include(ar => ar.ApprovedByUser);
			// Authorization: Apply user-specific filtering
			if (!hasApprovalPermission)
			{
				// If user doesn't have approval permission, only show their own requests
				query = query.Where(ar => ar.RequestorUserId == request.UserId);
			}
			// Apply filters
			if (request.Status.HasValue)
			{
				query = query.Where(ar => ar.Status == request.Status.Value);
			}
			if (request.RequestDateFromUtc.HasValue)
			{
				query = query.Where(ar => ar.RequestDateUtc >= request.RequestDateFromUtc.Value);
			}
			if (request.RequestDateToUtc.HasValue)
			{
				query = query.Where(ar => ar.RequestDateUtc <= request.RequestDateToUtc.Value);
			}
			if (!string.IsNullOrWhiteSpace(request.PatientName))
			{
				query = query.Where(ar => ar.PatientRecord.PatientName.Contains(request.PatientName));
			}
			// Get total count before pagination
			var totalCount = await query.CountAsync(cancellationToken);
			// Apply pagination
			var accessRequests = await query
				.OrderByDescending(ar => ar.RequestDateUtc)
				.Skip((request.PageNumber - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync(cancellationToken);
			var accessRequestResponses = accessRequests.Select(ar => new AccessRequestResponse(
				ar.Id,
				ar.PatientRecordId,
				ar.PatientRecord?.PatientName ?? "Unknown Patient",
				ar.RequestorUserId,
				ar.RequestorUser?.FullName ?? "Unknown User",
				ar.Reason,
				ar.RequestDateUtc,
				ar.Status,
				ar.ApprovedByUserId,
				ar.ApprovedByUser?.FullName,
				ar.ApprovedStartUtc,
				ar.ApprovedEndUtc,
				ar.DecisionDateUtc,
				ar.DeclineReason
			));
			var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
			return new AccessRequestsPagedResponse(
				accessRequestResponses,
				totalCount,
				request.PageNumber,
				request.PageSize,
				totalPages
			);
		}
	}
}