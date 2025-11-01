using System;
using System.Collections.Generic;
using MediatR;
using PHR.Application.DTOs;
namespace PHR.Application.Audit.Queries
{
	public record GetAuditLogsQuery(
		Guid RequestingUserId,
		Guid? UserId = null,
		string? EntityType = null,
		Guid? EntityId = null,
		DateTime? FromDate = null,
		DateTime? ToDate = null,
		int PageNumber = 1,
		int PageSize = 50
	) : IRequest<AuditLogsPagedResponse>;
	public record AuditLogsPagedResponse(
		IEnumerable<AuditLogResponse> AuditLogs,
		int TotalCount,
		int PageNumber,
		int PageSize,
		int TotalPages
	);
}