using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Data;
using PHR.Domain.Entities.Enums;
namespace PHR.Application.AccessRequests.Commands
{
	public class DeclineAccessRequestCommandHandler : IRequestHandler<DeclineAccessRequestCommand, bool>
	{
		private readonly IApplicationDbContext _context;
		public DeclineAccessRequestCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<bool> Handle(DeclineAccessRequestCommand request, CancellationToken cancellationToken)
		{
			var accessRequest = await _context.AccessRequests
				.FirstOrDefaultAsync(ar => ar.Id == request.AccessRequestId, cancellationToken);
			if (accessRequest == null)
			{
				throw new ApplicationException("Access request not found.");
			}
			if (accessRequest.Status != AccessRequestStatus.Pending)
			{
				throw new ApplicationException("Access request is not in pending status.");
			}
			accessRequest.Status = AccessRequestStatus.Declined;
			accessRequest.DeclineReason = request.DeclineReason;
			accessRequest.ApprovedByUserId = request.DeclinedByUserId; // Using same field for declined by
			accessRequest.DecisionDateUtc = DateTime.UtcNow;
			await _context.SaveChangesAsync(cancellationToken);
			return true;
		}
	}
}