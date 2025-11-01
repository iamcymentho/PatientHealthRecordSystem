using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Data;
using PHR.Application.DTOs;
using PHR.Domain.Entities.Enums;
namespace PHR.Application.AccessRequests.Queries
{
	public class GetPendingAccessRequestsQueryHandler : IRequestHandler<GetPendingAccessRequestsQuery, IEnumerable<AccessRequestResponse>>
	{
		private readonly IApplicationDbContext _context;
		public GetPendingAccessRequestsQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IEnumerable<AccessRequestResponse>> Handle(GetPendingAccessRequestsQuery request, CancellationToken cancellationToken)
		{
			var query = _context.AccessRequests
				.Include(ar => ar.RequestorUser)
				.Include(ar => ar.PatientRecord)
				.Include(ar => ar.ApprovedByUser)
				.Where(ar => ar.Status == AccessRequestStatus.Pending);
			// Check if user has approval permissions
			var hasApprovalPermission = await _context.Users
				.Where(u => u.Id == request.UserId)
				.SelectMany(u => u.UserRoles)
				.SelectMany(ur => ur.Role.RolePermissions)
				.AnyAsync(rp => rp.Permission.Name == "ApproveAccessRequests", cancellationToken);
			// If user doesn't have approval permission, they can only see their own requests
			if (!hasApprovalPermission)
			{
				query = query.Where(ar => ar.RequestorUserId == request.UserId);
			}
			var accessRequests = await query
				.OrderByDescending(ar => ar.RequestDateUtc)
				.ToListAsync(cancellationToken);
			return accessRequests.Select(ar => new AccessRequestResponse(
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
		}
	}
}