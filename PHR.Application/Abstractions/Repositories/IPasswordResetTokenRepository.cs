using System;
using System.Threading.Tasks;
using PHR.Domain.Entities;
namespace PHR.Application.Abstractions.Repositories
{
	public interface IPasswordResetTokenRepository
	{
		Task<PasswordResetToken?> GetByTokenAsync(string token);
		Task AddAsync(PasswordResetToken token);
		Task UpdateAsync(PasswordResetToken token);
		Task SaveChangesAsync();
	}
}