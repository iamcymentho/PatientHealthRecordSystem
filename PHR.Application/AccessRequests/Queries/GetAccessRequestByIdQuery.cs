using System;
using MediatR;
using PHR.Application.DTOs;
namespace PHR.Application.AccessRequests.Queries
{
	public record GetAccessRequestByIdQuery(
		Guid Id,
		Guid? RequestingUserId = null
	) : IRequest<AccessRequestResponse?>;
}