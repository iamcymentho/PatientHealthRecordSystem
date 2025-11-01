using System;
using MediatR;
namespace PHR.Application.PatientRecords.Commands
{
	public record UpdatePatientRecordCommand(
		Guid Id,
		string PatientName,
		DateTime DateOfBirth,
		string Diagnosis,
		string TreatmentPlan,
		string MedicalHistory,
		Guid ModifiedByUserId
	) : IRequest<bool>;
}