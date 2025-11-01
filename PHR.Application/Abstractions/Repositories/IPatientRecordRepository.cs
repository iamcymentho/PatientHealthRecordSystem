using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PHR.Domain.Entities;
namespace PHR.Application.Abstractions.Repositories
{
	public interface IPatientRecordRepository
	{
		Task<PatientRecord?> GetByIdAsync(Guid id);
		Task<IEnumerable<PatientRecord>> GetByUserIdAsync(Guid userId);
		Task<IEnumerable<PatientRecord>> GetAllAsync();
		Task<PatientRecord> CreateAsync(PatientRecord patientRecord);
		Task<PatientRecord> UpdateAsync(PatientRecord patientRecord);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> CanUserAccessRecordAsync(Guid userId, Guid recordId);
	}
}