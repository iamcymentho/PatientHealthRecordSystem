using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PHR.Application.DTOs;
namespace PHR.Application.Services.Interfaces
{
	public interface IPatientRecordService
	{
		Task<IActionResult> GetPatientRecordsAsync(Guid userId);
		Task<IActionResult> GetPatientRecordsWithFiltersAsync(Guid userId, string? patientName, DateTime? dateFrom, DateTime? dateTo, string? diagnosis, int pageNumber, int pageSize);
		Task<IActionResult> SearchPatientRecordsAsync(Guid userId, string searchTerm);
		Task<IActionResult> GetPatientRecordByIdAsync(Guid id, Guid userId);
		Task<IActionResult> CreatePatientRecordAsync(CreatePatientRecordRequest request, Guid userId);
		Task<IActionResult> UpdatePatientRecordAsync(Guid id, UpdatePatientRecordRequest request, Guid userId);
		Task<IActionResult> DeletePatientRecordAsync(Guid id, Guid userId);
	}
}
