using System;
using System.Collections.Generic;
using MediatR;
using PHR.Application.DTOs;
namespace PHR.Application.PatientRecords.Queries
{
	public record GetPatientRecordsWithFiltersQuery(
		Guid UserId,
		string? PatientName = null,
		DateTime? DateFromUtc = null,
		DateTime? DateToUtc = null,
		string? Diagnosis = null,
		int PageNumber = 1,
		int PageSize = 50
	) : IRequest<PatientRecordsPagedResponse>;
}
public record PatientRecordsPagedResponse(
	IEnumerable<PatientRecordResponse> Records,
	int TotalCount,
	int PageNumber,
	int PageSize,
	int TotalPages
);