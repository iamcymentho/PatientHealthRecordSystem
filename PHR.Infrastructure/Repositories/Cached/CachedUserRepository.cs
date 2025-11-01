using PHR.Application.Abstractions.Repositories;
using PHR.Application.Abstractions.Caching;
using PHR.Domain.Entities;
namespace PHR.Infrastructure.Repositories.Cached;
public class CachedUserRepository : IUserRepository
{
    private readonly UserRepository _decorated;
    private readonly ICacheService _cache;
    public CachedUserRepository(UserRepository decorated, ICacheService cache)
    {
        _decorated = decorated;
        _cache = cache;
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        // Don't cache by email as it's typically used for authentication
        return await _decorated.GetByEmailAsync(email);
    }
    public async Task<User?> GetByEmailWithRolesAsync(string email)
    {
        var cacheKey = $"user_with_roles_{email}";
        return await _cache.GetOrSetAsync(
            cacheKey,
            async () => await _decorated.GetByEmailWithRolesAsync(email),
            TimeSpan.FromMinutes(30)
        );
    }
    public async Task AddAsync(User user)
    {
        await _decorated.AddAsync(user);
        // Clear related caches
        await _cache.RemoveByPatternAsync("user_.*");
    }
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _decorated.EmailExistsAsync(email);
    }
    public async Task<User?> GetByIdAsync(Guid id)
    {
        var cacheKey = $"user_{id}";
        return await _cache.GetOrSetAsync(
            cacheKey,
            async () => await _decorated.GetByIdAsync(id),
            TimeSpan.FromMinutes(30)
        );
    }
    public async Task UpdateAsync(User user)
    {
        await _decorated.UpdateAsync(user);
        // Clear related caches
        await _cache.RemoveByPatternAsync("user_.*");
    }
    public async Task SaveChangesAsync()
    {
        await _decorated.SaveChangesAsync();
    }
}