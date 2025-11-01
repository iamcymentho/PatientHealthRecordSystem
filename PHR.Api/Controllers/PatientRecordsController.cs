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
	public class PatientRecordsController : BaseController
	{
		private readonly IPatientRecordService _patientRecordService;
		public PatientRecordsController(IPatientRecordService patientRecordService)
		{
			_patientRecordService = patientRecordService;
		}
		[HttpGet]
		public async Task<IActionResult> GetPatientRecords()
		{
			var userId = GetCurrentUserId();
			var response = await _patientRecordService.GetPatientRecordsAsync(userId);
			return Ok(response);
		}
		[HttpGet("filtered")]
		public async Task<IActionResult> GetPatientRecordsFiltered(
			[FromQuery] string? patientName = null,
			[FromQuery] DateTime? dateFrom = null,
			[FromQuery] DateTime? dateTo = null,
			[FromQuery] string? diagnosis = null,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 50)
		{
			var userId = GetCurrentUserId();
			var response = await _patientRecordService.GetPatientRecordsWithFiltersAsync(
				userId, patientName, dateFrom, dateTo, diagnosis, pageNumber, pageSize);
			return Ok(response);
		}
		[HttpGet("search")]
		public async Task<IActionResult> SearchPatientRecords([FromQuery] string searchTerm)
		{
			var userId = GetCurrentUserId();
			var response = await _patientRecordService.SearchPatientRecordsAsync(userId, searchTerm);
			return Ok(response);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetPatientRecord(Guid id)
		{
			var userId = GetCurrentUserId();
			var response = await _patientRecordService.GetPatientRecordByIdAsync(id, userId);
			return Ok(response);
		}
		[HttpPost]
		[Authorize(Policy = AuthorizationPolicies.CreatePatientRecords)]
		public async Task<IActionResult> CreatePatientRecord([FromBody] CreatePatientRecordRequest request)
		{
			var userId = GetCurrentUserId();
			var response = await _patientRecordService.CreatePatientRecordAsync(request, userId);
			return Ok(response);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdatePatientRecord(Guid id, [FromBody] UpdatePatientRecordRequest request)
		{
			var userId = GetCurrentUserId();
			var response = await _patientRecordService.UpdatePatientRecordAsync(id, request, userId);
			return Ok(response);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePatientRecord(Guid id)
		{
			var userId = GetCurrentUserId();
			var response = await _patientRecordService.DeletePatientRecordAsync(id, userId);
			return Ok(response);
		}
	}
}