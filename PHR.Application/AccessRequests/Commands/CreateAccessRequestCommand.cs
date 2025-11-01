using System;
using MediatR;
namespace PHR.Application.AccessRequests.Commands
{
	public record CreateAccessRequestCommand(
		Guid PatientRecordId,
		string Reason,
		Guid RequestorUserId
	) : IRequest<Guid>;
}