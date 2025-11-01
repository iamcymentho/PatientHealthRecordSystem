using System;
using MediatR;
namespace PHR.Application.PatientRecords.Commands
{
	public record CreatePatientRecordCommand(
		string PatientName,
		DateTime DateOfBirth,
		string Diagnosis,
		string TreatmentPlan,
		string MedicalHistory,
		Guid CreatedByUserId
	) : IRequest<Guid>;
}