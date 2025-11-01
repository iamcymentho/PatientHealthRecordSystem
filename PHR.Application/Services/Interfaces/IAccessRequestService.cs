using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PHR.Application.DTOs;
namespace PHR.Application.Services.Interfaces
{
	public interface IAccessRequestService
	{
		Task<IActionResult> GetPendingAccessRequestsAsync(Guid userId);
		Task<IActionResult> GetAccessRequestByIdAsync(Guid id, Guid userId);
		Task<IActionResult> CreateAccessRequestAsync(CreateAccessRequestDto request, Guid userId);
		Task<IActionResult> ApproveAccessRequestAsync(Guid id, ApproveAccessRequestDto request, Guid userId);
		Task<IActionResult> DeclineAccessRequestAsync(Guid id, DeclineAccessRequestDto request, Guid userId);
	}
}
