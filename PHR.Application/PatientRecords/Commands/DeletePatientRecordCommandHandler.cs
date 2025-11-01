using MediatR;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Data;
namespace PHR.Application.PatientRecords.Commands
{
	public class DeletePatientRecordCommandHandler : IRequestHandler<DeletePatientRecordCommand, bool>
	{
		private readonly IApplicationDbContext _context;
		public DeletePatientRecordCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<bool> Handle(DeletePatientRecordCommand request, CancellationToken cancellationToken)
		{
			var patientRecord = await _context.PatientRecords
				.IgnoreQueryFilters() // Include soft-deleted records to check ownership
				.FirstOrDefaultAsync(pr => pr.Id == request.Id, cancellationToken);
			if (patientRecord == null)
			{
				throw new ApplicationException("Patient record not found.");
			}
			if (patientRecord.IsDeleted)
			{
				throw new ApplicationException("Patient record is already deleted.");
			}
			// Check if the user can delete this record (must be the creator)
			if (patientRecord.CreatedByUserId != request.DeletedByUserId)
			{
				throw new UnauthorizedAccessException("You can only delete records you created.");
			}
			patientRecord.IsDeleted = true;
			patientRecord.LastModifiedByUserId = request.DeletedByUserId;
			patientRecord.LastModifiedDateUtc = DateTime.UtcNow;
			await _context.SaveChangesAsync(cancellationToken);
			return true;
		}
	}
}