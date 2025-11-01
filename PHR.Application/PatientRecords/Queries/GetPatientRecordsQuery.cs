using System;
using System.Collections.Generic;
using MediatR;
using PHR.Application.DTOs;
namespace PHR.Application.PatientRecords.Queries
{
	public record GetPatientRecordsQuery(
		Guid UserId
	) : IRequest<IEnumerable<PatientRecordResponse>>;
}