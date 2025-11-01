using System;
using MediatR;
using PHR.Application.DTOs;
namespace PHR.Application.PatientRecords.Queries
{
	public record GetPatientRecordByIdQuery(
		Guid Id,
		Guid UserId
	) : IRequest<PatientRecordResponse?>;
}