using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Repositories;
using PHR.Domain.Entities;
namespace PHR.Infrastructure.Repositories
{
	public class PatientRecordRepository : IPatientRecordRepository
	{
		private readonly PHRDbContext _context;
		public PatientRecordRepository(PHRDbContext context)
		{
			_context = context;
		}
		public async Task<PatientRecord?> GetByIdAsync(Guid id)
		{
			return await _context.PatientRecords
				.Include(pr => pr.CreatedByUser)
				.Include(pr => pr.LastModifiedByUser)
				.FirstOrDefaultAsync(pr => pr.Id == id);
		}
		public async Task<IEnumerable<PatientRecord>> GetByUserIdAsync(Guid userId)
		{
			return await _context.PatientRecords
				.Include(pr => pr.CreatedByUser)
				.Where(pr => pr.CreatedByUserId == userId)
				.OrderByDescending(pr => pr.CreatedDateUtc)
				.ToListAsync();
		}
		public async Task<IEnumerable<PatientRecord>> GetAllAsync()
		{
			return await _context.PatientRecords
				.Include(pr => pr.CreatedByUser)
				.OrderByDescending(pr => pr.CreatedDateUtc)
				.ToListAsync();
		}
		public async Task<PatientRecord> CreateAsync(PatientRecord patientRecord)
		{
			_context.PatientRecords.Add(patientRecord);
			await _context.SaveChangesAsync();
			return patientRecord;
		}
		public async Task<PatientRecord> UpdateAsync(PatientRecord patientRecord)
		{
			_context.PatientRecords.Update(patientRecord);
			await _context.SaveChangesAsync();
			return patientRecord;
		}
		public async Task<bool> DeleteAsync(Guid id)
		{
			var patientRecord = await _context.PatientRecords
				.IgnoreQueryFilters()
				.FirstOrDefaultAsync(pr => pr.Id == id);
			if (patientRecord == null || patientRecord.IsDeleted)
			{
				return false;
			}
			patientRecord.IsDeleted = true;
			patientRecord.LastModifiedDateUtc = DateTime.UtcNow;
			await _context.SaveChangesAsync();
			return true;
		}
		public async Task<bool> CanUserAccessRecordAsync(Guid userId, Guid recordId)
		{
			var record = await _context.PatientRecords
				.FirstOrDefaultAsync(pr => pr.Id == recordId);
			if (record == null)
				return false;
			// User can access if they created the record
			if (record.CreatedByUserId == userId)
				return true;
			// TODO: Check for approved access requests
			var approvedAccess = await _context.AccessRequests
				.AnyAsync(ar => ar.PatientRecordId == recordId &&
				               ar.RequestorUserId == userId &&
				               ar.Status == Domain.Entities.Enums.AccessRequestStatus.Approved &&
				               ar.ApprovedStartUtc <= DateTime.UtcNow &&
				               ar.ApprovedEndUtc >= DateTime.UtcNow);
			return approvedAccess;
		}
	}
}