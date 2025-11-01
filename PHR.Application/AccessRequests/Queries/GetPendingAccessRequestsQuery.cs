using System;
using System.Collections.Generic;
using MediatR;
using PHR.Application.DTOs;
namespace PHR.Application.AccessRequests.Queries
{
	public record GetPendingAccessRequestsQuery(
		Guid UserId // The requesting user ID - handler will determine what they can see
	) : IRequest<IEnumerable<AccessRequestResponse>>;
}