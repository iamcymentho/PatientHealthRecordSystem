using System;
using MediatR;
namespace PHR.Application.AccessRequests.Commands
{
	public record DeclineAccessRequestCommand(
		Guid AccessRequestId,
		string DeclineReason,
		Guid DeclinedByUserId
	) : IRequest<bool>;
}