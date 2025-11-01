using System;
using MediatR;
namespace PHR.Application.AccessRequests.Commands
{
	public record ApproveAccessRequestCommand(
		Guid AccessRequestId,
		DateTime ApprovedStartUtc,
		DateTime ApprovedEndUtc,
		Guid ApprovedByUserId
	) : IRequest<bool>;
}