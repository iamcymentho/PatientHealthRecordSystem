using System;
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
	public class GetAccessRequestByIdQueryHandler : IRequestHandler<GetAccessRequestByIdQuery, AccessRequestResponse?>
	{
		private readonly IApplicationDbContext _context;
		public GetAccessRequestByIdQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<AccessRequestResponse?> Handle(GetAccessRequestByIdQuery request, CancellationToken cancellationToken)
		{
			var query = _context.AccessRequests
				.Include(ar => ar.RequestorUser)
				.Include(ar => ar.PatientRecord)
				.Include(ar => ar.ApprovedByUser)
				.Where(ar => ar.Id == request.Id);
			// Authorization: Users can only view their own access requests
			// OR users with ApproveAccessRequests permission can view any request
			if (request.RequestingUserId.HasValue)
			{
				var userId = request.RequestingUserId.Value;
				// Check if user has approval permissions
				var hasApprovalPermission = await _context.Users
					.Where(u => u.Id == userId)
					.SelectMany(u => u.UserRoles)
					.SelectMany(ur => ur.Role.RolePermissions)
					.AnyAsync(rp => rp.Permission.Name == Permissions.ApproveAccessRequests, cancellationToken);
				// If user doesn't have approval permission, they can only see their own requests
				if (!hasApprovalPermission)
				{
					query = query.Where(ar => ar.RequestorUserId == userId);
				}
			}
			var accessRequest = await query.FirstOrDefaultAsync(cancellationToken);
			if (accessRequest == null)
			{
				return null;
			}
			return new AccessRequestResponse(
				accessRequest.Id,
				accessRequest.PatientRecordId,
				accessRequest.PatientRecord?.PatientName ?? "Unknown Patient",
				accessRequest.RequestorUserId,
				accessRequest.RequestorUser?.FullName ?? "Unknown User",
				accessRequest.Reason,
				accessRequest.RequestDateUtc,
				accessRequest.Status,
				accessRequest.ApprovedByUserId,
				accessRequest.ApprovedByUser?.FullName,
				accessRequest.ApprovedStartUtc,
				accessRequest.ApprovedEndUtc,
				accessRequest.DecisionDateUtc,
				accessRequest.DeclineReason
			);
		}
	}
}