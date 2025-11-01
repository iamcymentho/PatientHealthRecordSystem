using MediatR;
using PHR.Application.DTOs;
using PHR.Domain.Entities.Enums;
namespace PHR.Application.AccessRequests.Queries
{
	public record GetAccessRequestsWithFiltersQuery(
		Guid UserId,
		AccessRequestStatus? Status = null,
		DateTime? RequestDateFromUtc = null,
		DateTime? RequestDateToUtc = null,
		string? PatientName = null,
		int PageNumber = 1,
		int PageSize = 50
	) : IRequest<AccessRequestsPagedResponse>;
}
public record AccessRequestsPagedResponse(
	IEnumerable<AccessRequestResponse> AccessRequests,
	int TotalCount,
	int PageNumber,
	int PageSize,
	int TotalPages
);