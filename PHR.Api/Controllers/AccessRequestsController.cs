using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PHR.Application.Services.Interfaces;
using PHR.Application.DTOs;
using PHR.Infrastructure.Authorization;
namespace PHR.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class AccessRequestsController : BaseController
	{
		private readonly IAccessRequestService _accessRequestService;
		public AccessRequestsController(IAccessRequestService accessRequestService)
		{
			_accessRequestService = accessRequestService;
		}
		[HttpGet("pending")]
		public async Task<IActionResult> GetPendingAccessRequests()
		{
			var userId = GetCurrentUserId();
			var response = await _accessRequestService.GetPendingAccessRequestsAsync(userId);
			return Ok(response);
		}
		[HttpPost]
		public async Task<IActionResult> CreateAccessRequest([FromBody] CreateAccessRequestDto request)
		{
			var userId = GetCurrentUserId();
			var response = await _accessRequestService.CreateAccessRequestAsync(request, userId);
			return Ok(response);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetAccessRequest(Guid id)
		{
			var userId = GetCurrentUserId();
			var response = await _accessRequestService.GetAccessRequestByIdAsync(id, userId);
			return Ok(response);
		}
		[HttpPost("{id}/approve")]
		[Authorize(Policy = AuthorizationPolicies.ApproveAccessRequests)]
		public async Task<IActionResult> ApproveAccessRequest(Guid id, [FromBody] ApproveAccessRequestDto request)
		{
			var userId = GetCurrentUserId();
			var response = await _accessRequestService.ApproveAccessRequestAsync(id, request, userId);
			return Ok(response);
		}
		[HttpPost("{id}/decline")]
		[Authorize(Policy = AuthorizationPolicies.ApproveAccessRequests)]
		public async Task<IActionResult> DeclineAccessRequest(Guid id, [FromBody] DeclineAccessRequestDto request)
		{
			var userId = GetCurrentUserId();
			var response = await _accessRequestService.DeclineAccessRequestAsync(id, request, userId);
			return Ok(response);
		}
	}
}