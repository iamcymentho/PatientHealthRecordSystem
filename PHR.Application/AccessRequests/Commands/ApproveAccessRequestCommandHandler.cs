using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Data;
using PHR.Application.Abstractions.Services;
using PHR.Domain.Entities;
using PHR.Domain.Entities.Enums;
namespace PHR.Application.AccessRequests.Commands
{
	public class ApproveAccessRequestCommandHandler : IRequestHandler<ApproveAccessRequestCommand, bool>
	{
		private readonly IApplicationDbContext _context;
		private readonly IAuditService _auditService;
		private readonly IEmailService _emailService;
		public ApproveAccessRequestCommandHandler(
			IApplicationDbContext context,
			IAuditService auditService,
			IEmailService emailService)
		{
			_context = context;
			_auditService = auditService;
			_emailService = emailService;
		}
		public async Task<bool> Handle(ApproveAccessRequestCommand request, CancellationToken cancellationToken)
		{
			var accessRequest = await _context.AccessRequests
				.Include(ar => ar.RequestorUser)
				.Include(ar => ar.PatientRecord)
				.Include(ar => ar.ApprovedByUser)
				.FirstOrDefaultAsync(ar => ar.Id == request.AccessRequestId, cancellationToken);
			if (accessRequest == null)
			{
				throw new ApplicationException("Access request not found.");
			}
			if (accessRequest.Status != AccessRequestStatus.Pending)
			{
				throw new ApplicationException("Access request is not in pending status.");
			}
			// Validate date range
			if (request.ApprovedStartUtc >= request.ApprovedEndUtc)
			{
				throw new ApplicationException("Approved start date must be before end date.");
			}
			if (request.ApprovedEndUtc <= DateTime.UtcNow)
			{
				throw new ApplicationException("Approved end date must be in the future.");
			}
			accessRequest.Status = AccessRequestStatus.Approved;
			accessRequest.ApprovedByUserId = request.ApprovedByUserId;
			accessRequest.ApprovedStartUtc = request.ApprovedStartUtc;
			accessRequest.ApprovedEndUtc = request.ApprovedEndUtc;
			accessRequest.DecisionDateUtc = DateTime.UtcNow;
			await _context.SaveChangesAsync(cancellationToken);
			// Get approver information
			var approver = await _context.Users
				.FirstOrDefaultAsync(u => u.Id == request.ApprovedByUserId, cancellationToken);
			// Log audit trail
			await _auditService.LogAccessRequestActionAsync(
				request.ApprovedByUserId,
				accessRequest.Id,
				AuditActions.AccessRequestApproved,
				$"Approved access to {accessRequest.PatientRecord?.PatientName} from {request.ApprovedStartUtc:yyyy-MM-dd} to {request.ApprovedEndUtc:yyyy-MM-dd}");
			// Send email notification to requestor
			if (accessRequest.RequestorUser != null && !string.IsNullOrEmpty(accessRequest.RequestorUser.Email))
			{
				await _emailService.SendAccessRequestApprovedAsync(
					accessRequest.RequestorUser.Email,
					accessRequest.RequestorUser.FullName,
					accessRequest.PatientRecord?.PatientName ?? "Unknown Patient",
					approver?.FullName ?? "Administrator",
					request.ApprovedStartUtc.ToString("yyyy-MM-dd HH:mm UTC"),
					request.ApprovedEndUtc.ToString("yyyy-MM-dd HH:mm UTC"));
			}
			return true;
		}
	}
}