using FluentValidation;
using System;
namespace PHR.Application.PatientRecords.Commands
{
	public class CreatePatientRecordCommandValidator : AbstractValidator<CreatePatientRecordCommand>
	{
		public CreatePatientRecordCommandValidator()
		{
			RuleFor(x => x.PatientName)
				.NotEmpty()
				.MaximumLength(200)
				.WithMessage("Patient name is required and must be less than 200 characters.");
			RuleFor(x => x.DateOfBirth)
				.NotEmpty()
				.LessThan(DateTime.Now)
				.WithMessage("Date of birth must be in the past.");
			RuleFor(x => x.Diagnosis)
				.NotEmpty()
				.MaximumLength(1000)
				.WithMessage("Diagnosis is required and must be less than 1000 characters.");
			RuleFor(x => x.TreatmentPlan)
				.NotEmpty()
				.MaximumLength(2000)
				.WithMessage("Treatment plan is required and must be less than 2000 characters.");
			RuleFor(x => x.MedicalHistory)
				.NotEmpty()
				.MaximumLength(5000)
				.WithMessage("Medical history is required and must be less than 5000 characters.");
			RuleFor(x => x.CreatedByUserId)
				.NotEmpty()
				.WithMessage("Created by user ID is required.");
		}
	}
}