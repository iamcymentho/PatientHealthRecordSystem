using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Data;
using PHR.Domain.Entities;
using PHR.Domain.Entities.Enums;
namespace PHR.Application.AccessRequests.Commands
{
	public class CreateAccessRequestCommandHandler : IRequestHandler<CreateAccessRequestCommand, Guid>
	{
		private readonly IApplicationDbContext _context;
		public CreateAccessRequestCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<Guid> Handle(CreateAccessRequestCommand request, CancellationToken cancellationToken)
		{
			// Check if patient record exists
			var patientRecord = await _context.PatientRecords
				.FirstOrDefaultAsync(pr => pr.Id == request.PatientRecordId, cancellationToken);
			if (patientRecord == null)
			{
				throw new ApplicationException("Patient record not found.");
			}
			// Check if user already created this record (shouldn't need to request access)
			if (patientRecord.CreatedByUserId == request.RequestorUserId)
			{
				throw new ApplicationException("You cannot request access to a record you created.");
			}
			// Check for existing pending request
			var existingRequest = await _context.AccessRequests
				.FirstOrDefaultAsync(ar => ar.PatientRecordId == request.PatientRecordId &&
				                          ar.RequestorUserId == request.RequestorUserId &&
				                          ar.Status == AccessRequestStatus.Pending, cancellationToken);
			if (existingRequest != null)
			{
				throw new ApplicationException("You already have a pending access request for this record.");
			}
			var accessRequest = new AccessRequest
			{
				Id = Guid.NewGuid(),
				PatientRecordId = request.PatientRecordId,
				RequestorUserId = request.RequestorUserId,
				Reason = request.Reason,
				RequestDateUtc = DateTime.UtcNow,
				Status = AccessRequestStatus.Pending
			};
			_context.AccessRequests.Add(accessRequest);
			await _context.SaveChangesAsync(cancellationToken);
			return accessRequest.Id;
		}
	}
}