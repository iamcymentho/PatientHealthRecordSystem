using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Data;
namespace PHR.Application.PatientRecords.Commands
{
	public class UpdatePatientRecordCommandHandler : IRequestHandler<UpdatePatientRecordCommand, bool>
	{
		private readonly IApplicationDbContext _context;
		public UpdatePatientRecordCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<bool> Handle(UpdatePatientRecordCommand request, CancellationToken cancellationToken)
		{
			var patientRecord = await _context.PatientRecords
				.FirstOrDefaultAsync(pr => pr.Id == request.Id, cancellationToken);
			if (patientRecord == null)
			{
				throw new ApplicationException("Patient record not found.");
			}
			// Check if the user can modify this record (must be the creator)
			if (patientRecord.CreatedByUserId != request.ModifiedByUserId)
			{
				throw new UnauthorizedAccessException("You can only update records you created.");
			}
			patientRecord.PatientName = request.PatientName;
			patientRecord.DateOfBirth = request.DateOfBirth;
			patientRecord.Diagnosis = request.Diagnosis;
			patientRecord.TreatmentPlan = request.TreatmentPlan;
			patientRecord.MedicalHistory = request.MedicalHistory;
			patientRecord.LastModifiedByUserId = request.ModifiedByUserId;
			patientRecord.LastModifiedDateUtc = DateTime.UtcNow;
			await _context.SaveChangesAsync(cancellationToken);
			return true;
		}
	}
}