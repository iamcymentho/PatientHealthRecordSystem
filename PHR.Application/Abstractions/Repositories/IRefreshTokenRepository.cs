using System;
using System.Threading.Tasks;
using PHR.Domain.Entities;
namespace PHR.Application.Abstractions.Repositories
{
	public interface IRefreshTokenRepository
	{
		Task<RefreshToken?> GetByTokenAsync(string token);
		Task AddAsync(RefreshToken refreshToken);
		Task UpdateAsync(RefreshToken refreshToken);
		Task SaveChangesAsync();
	}
}