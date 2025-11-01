using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PHR.Application.Services.Interfaces;
using PHR.Application.DTOs;
using PHR.Application.PatientRecords.Commands;
using PHR.Application.PatientRecords.Queries;
namespace PHR.Application.Services.Implementation
{
	public class PatientRecordService : ControllerBase, IPatientRecordService
	{
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;
		public PatientRecordService(IMediator mediator, IMapper mapper)
		{
			_mediator = mediator;
			_mapper = mapper;
		}
		public async Task<IActionResult> GetPatientRecordsAsync(Guid userId)
		{
			var query = new GetPatientRecordsQuery(userId);
			var records = await _mediator.Send(query);
			return Ok(records);
		}
		public async Task<IActionResult> GetPatientRecordsWithFiltersAsync(
			Guid userId,
			string? patientName,
			DateTime? dateFrom,
			DateTime? dateTo,
			string? diagnosis,
			int pageNumber,
			int pageSize)
		{
			var query = new GetPatientRecordsWithFiltersQuery(
				userId,
				patientName,
				dateFrom,
				dateTo,
				diagnosis,
				pageNumber,
				pageSize);
			var result = await _mediator.Send(query);
			return Ok(result);
		}
		public async Task<IActionResult> SearchPatientRecordsAsync(Guid userId, string searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				return BadRequest("Search term is required");
			}
			var query = new SearchPatientRecordsQuery(userId, searchTerm);
			var records = await _mediator.Send(query);
			return Ok(records);
		}
		public async Task<IActionResult> GetPatientRecordByIdAsync(Guid id, Guid userId)
		{
			var query = new GetPatientRecordByIdQuery(id, userId);
			var record = await _mediator.Send(query);
			if (record == null)
			{
				return NotFound();
			}
			return Ok(record);
		}
		public async Task<IActionResult> CreatePatientRecordAsync(CreatePatientRecordRequest request, Guid userId)
		{
			var command = _mapper.Map<CreatePatientRecordCommand>(request, opts =>
				opts.Items["UserId"] = userId);
			var patientRecordId = await _mediator.Send(command);
			return CreatedAtAction("GetPatientRecord", new { id = patientRecordId }, new { id = patientRecordId });
		}
		public async Task<IActionResult> UpdatePatientRecordAsync(Guid id, UpdatePatientRecordRequest request, Guid userId)
		{
			var command = _mapper.Map<UpdatePatientRecordCommand>(request, opts =>
			{
				opts.Items["Id"] = id;
				opts.Items["UserId"] = userId;
			});
			var result = await _mediator.Send(command);
			return Ok(new { success = result });
		}
		public async Task<IActionResult> DeletePatientRecordAsync(Guid id, Guid userId)
		{
			var command = new DeletePatientRecordCommand(id, userId);
			var result = await _mediator.Send(command);
			return Ok(new { success = result });
		}
	}
}
