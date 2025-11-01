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
namespace PHR.Application.Audit.Queries
{
	public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, AuditLogsPagedResponse>
	{
		private readonly IApplicationDbContext _context;
		public GetAuditLogsQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<AuditLogsPagedResponse> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
		{
			// Check if requesting user has ViewAuditLogs permission (Admin only)
			var hasViewAuditPermission = await _context.Users
				.Where(u => u.Id == request.RequestingUserId)
				.SelectMany(u => u.UserRoles)
				.SelectMany(ur => ur.Role.RolePermissions)
				.AnyAsync(rp => rp.Permission.Name == Permissions.ViewAuditLogs, cancellationToken);
			if (!hasViewAuditPermission)
			{
				throw new UnauthorizedAccessException("You don't have permission to view audit logs.");
			}
			IQueryable<Domain.Entities.AuditLog> query = _context.AuditLogs
				.Include(al => al.User);
			// Apply filters
			if (request.UserId.HasValue)
			{
				query = query.Where(al => al.UserId == request.UserId.Value);
			}
			if (!string.IsNullOrWhiteSpace(request.EntityType))
			{
				query = query.Where(al => al.EntityType == request.EntityType);
			}
			if (request.EntityId.HasValue)
			{
				query = query.Where(al => al.EntityId == request.EntityId.Value);
			}
			if (request.FromDate.HasValue)
			{
				query = query.Where(al => al.TimestampUtc >= request.FromDate.Value);
			}
			if (request.ToDate.HasValue)
			{
				query = query.Where(al => al.TimestampUtc <= request.ToDate.Value);
			}
			// Get total count before pagination
			var totalCount = await query.CountAsync(cancellationToken);
			// Apply pagination
			var auditLogs = await query
				.OrderByDescending(al => al.TimestampUtc)
				.Skip((request.PageNumber - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync(cancellationToken);
			var auditLogResponses = auditLogs.Select(al => new AuditLogResponse(
				al.Id,
				al.UserId,
				al.User?.FullName ?? "Unknown User",
				al.Action,
				al.EntityType,
				al.EntityId,
				al.Details,
				al.TimestampUtc,
				al.IpAddress
			));
			var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
			return new AuditLogsPagedResponse(
				auditLogResponses,
				totalCount,
				request.PageNumber,
				request.PageSize,
				totalPages
			);
		}
	}
}