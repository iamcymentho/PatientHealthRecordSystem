using MediatR;
using PHR.Application.Abstractions.Data;
using PHR.Domain.Entities;
namespace PHR.Application.PatientRecords.Commands
{
	public class CreatePatientRecordCommandHandler : IRequestHandler<CreatePatientRecordCommand, Guid>
	{
		private readonly IApplicationDbContext _context;
		public CreatePatientRecordCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<Guid> Handle(CreatePatientRecordCommand request, CancellationToken cancellationToken)
		{
			var patientRecord = new PatientRecord
			{
				Id = Guid.NewGuid(),
				PatientName = request.PatientName,
				DateOfBirth = request.DateOfBirth,
				Diagnosis = request.Diagnosis,
				TreatmentPlan = request.TreatmentPlan,
				MedicalHistory = request.MedicalHistory,
				CreatedByUserId = request.CreatedByUserId,
				CreatedDateUtc = DateTime.UtcNow,
				IsDeleted = false
			};
			_context.PatientRecords.Add(patientRecord);
			await _context.SaveChangesAsync(cancellationToken);
			return patientRecord.Id;
		}
	}
}