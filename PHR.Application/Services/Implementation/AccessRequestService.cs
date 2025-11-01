using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PHR.Application.Services.Interfaces;
using PHR.Application.AccessRequests.Commands;
using PHR.Application.AccessRequests.Queries;
using PHR.Application.DTOs;
namespace PHR.Application.Services.Implementation
{
	public class AccessRequestService : ControllerBase, IAccessRequestService
	{
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;
		public AccessRequestService(IMediator mediator, IMapper mapper)
		{
			_mediator = mediator;
			_mapper = mapper;
		}
		public async Task<IActionResult> GetPendingAccessRequestsAsync(Guid userId)
		{
			var query = new GetPendingAccessRequestsQuery(userId);
			var result = await _mediator.Send(query);
			return Ok(result);
		}
		public async Task<IActionResult> GetAccessRequestByIdAsync(Guid id, Guid userId)
		{
			var query = new GetAccessRequestByIdQuery(id, userId);
			var accessRequest = await _mediator.Send(query);
			if (accessRequest == null)
			{
				return NotFound("Access request not found.");
			}
			return Ok(accessRequest);
		}
		public async Task<IActionResult> CreateAccessRequestAsync(CreateAccessRequestDto request, Guid userId)
		{
			try
			{
				var command = _mapper.Map<CreateAccessRequestCommand>(request, opts =>
					opts.Items["UserId"] = userId);
				var accessRequestId = await _mediator.Send(command);
				return CreatedAtAction("GetAccessRequest", new { id = accessRequestId }, new { id = accessRequestId });
			}
			catch (ApplicationException ex)
			{
				return BadRequest(ex.Message);
			}
		}
		public async Task<IActionResult> ApproveAccessRequestAsync(Guid id, ApproveAccessRequestDto request, Guid userId)
		{
			try
			{
				var command = _mapper.Map<ApproveAccessRequestCommand>(request, opts =>
				{
					opts.Items["AccessRequestId"] = id;
					opts.Items["UserId"] = userId;
				});
				var result = await _mediator.Send(command);
				return Ok(new { success = result });
			}
			catch (ApplicationException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Forbid(ex.Message);
			}
		}
		public async Task<IActionResult> DeclineAccessRequestAsync(Guid id, DeclineAccessRequestDto request, Guid userId)
		{
			try
			{
				var command = _mapper.Map<DeclineAccessRequestCommand>(request, opts =>
				{
					opts.Items["AccessRequestId"] = id;
					opts.Items["UserId"] = userId;
				});
				var result = await _mediator.Send(command);
				return Ok(new { success = result });
			}
			catch (ApplicationException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Forbid(ex.Message);
			}
		}
	}
}
