using PHR.Application.Abstractions.Repositories;
using PHR.Application.Abstractions.Caching;
using PHR.Domain.Entities;
namespace PHR.Infrastructure.Repositories.Cached;
public class CachedPatientRecordRepository : IPatientRecordRepository
{
    private readonly PatientRecordRepository _decorated;
    private readonly ICacheService _cache;
    public CachedPatientRecordRepository(PatientRecordRepository decorated, ICacheService cache)
    {
        _decorated = decorated;
        _cache = cache;
    }
    public async Task<PatientRecord?> GetByIdAsync(Guid id)
    {
        var cacheKey = $"patient_record_{id}";
        return await _cache.GetOrSetAsync(
            cacheKey,
            async () => await _decorated.GetByIdAsync(id),
            TimeSpan.FromMinutes(15) // Shorter cache for patient records due to sensitivity
        );
    }
    public async Task<IEnumerable<PatientRecord>> GetByUserIdAsync(Guid userId)
    {
        var cacheKey = $"patient_records_user_{userId}";
        return await _cache.GetOrSetAsync(
            cacheKey,
            async () => await _decorated.GetByUserIdAsync(userId),
            TimeSpan.FromMinutes(10)
        ) ?? Enumerable.Empty<PatientRecord>();
    }
    public async Task<IEnumerable<PatientRecord>> GetAllAsync()
    {
        return await _decorated.GetAllAsync();
    }
    public async Task<PatientRecord> CreateAsync(PatientRecord patientRecord)
    {
        var result = await _decorated.CreateAsync(patientRecord);
        // Clear related caches
        await _cache.RemoveByPatternAsync("patient_record.*");
        return result;
    }
    public async Task<PatientRecord> UpdateAsync(PatientRecord patientRecord)
    {
        var result = await _decorated.UpdateAsync(patientRecord);
        // Clear specific patient record cache and user records cache
        await _cache.RemoveAsync($"patient_record_{patientRecord.Id}");
        await _cache.RemoveAsync($"patient_records_user_{patientRecord.CreatedByUserId}");
        return result;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _decorated.DeleteAsync(id);
        // Clear specific patient record cache
        await _cache.RemoveAsync($"patient_record_{id}");
        await _cache.RemoveByPatternAsync("patient_records_user_.*");
        return result;
    }
    public async Task<bool> CanUserAccessRecordAsync(Guid userId, Guid recordId)
    {
        return await _decorated.CanUserAccessRecordAsync(userId, recordId);
    }
}