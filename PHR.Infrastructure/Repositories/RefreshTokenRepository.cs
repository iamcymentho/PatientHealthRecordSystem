using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Repositories;
using PHR.Domain.Entities;
namespace PHR.Infrastructure.Repositories
{
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		private readonly PHRDbContext _context;
		public RefreshTokenRepository(PHRDbContext context)
		{
			_context = context;
		}
		public async Task<RefreshToken?> GetByTokenAsync(string token)
		{
			return await _context.RefreshTokens
				.Include(rt => rt.User)
					.ThenInclude(u => u.UserRoles)
						.ThenInclude(ur => ur.Role)
							.ThenInclude(r => r.RolePermissions)
								.ThenInclude(rp => rp.Permission)
				.FirstOrDefaultAsync(rt => rt.Token == token);
		}
		public async Task AddAsync(RefreshToken refreshToken)
		{
			await _context.RefreshTokens.AddAsync(refreshToken);
		}
		public Task UpdateAsync(RefreshToken refreshToken)
		{
			_context.RefreshTokens.Update(refreshToken);
			return Task.CompletedTask;
		}
		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}