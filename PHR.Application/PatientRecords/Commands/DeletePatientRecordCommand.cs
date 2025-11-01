using System;
using MediatR;
namespace PHR.Application.PatientRecords.Commands
{
	public record DeletePatientRecordCommand(
		Guid Id,
		Guid DeletedByUserId
	) : IRequest<bool>;
}