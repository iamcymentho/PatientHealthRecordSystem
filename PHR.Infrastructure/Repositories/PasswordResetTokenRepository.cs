using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Repositories;
using PHR.Domain.Entities;
namespace PHR.Infrastructure.Repositories
{
	public class PasswordResetTokenRepository : IPasswordResetTokenRepository
	{
		private readonly PHRDbContext _context;
		public PasswordResetTokenRepository(PHRDbContext context)
		{
			_context = context;
		}
		public Task<PasswordResetToken?> GetByTokenAsync(string token)
		{
			return _context.PasswordResetTokens
				.Include(prt => prt.User)
				.FirstOrDefaultAsync(prt => prt.Token == token);
		}
		public async Task AddAsync(PasswordResetToken token)
		{
			await _context.PasswordResetTokens.AddAsync(token);
		}
		public Task UpdateAsync(PasswordResetToken token)
		{
			_context.PasswordResetTokens.Update(token);
			return Task.CompletedTask;
		}
		public Task SaveChangesAsync()
		{
			return _context.SaveChangesAsync();
		}
	}
}