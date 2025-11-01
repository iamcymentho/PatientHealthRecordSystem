using System;
using System.Threading.Tasks;
using PHR.Domain.Entities;
namespace PHR.Application.Abstractions.Repositories
{
	public interface IUserRepository
	{
		Task<User?> GetByIdAsync(Guid id);
		Task<User?> GetByEmailAsync(string email);
		Task<User?> GetByEmailWithRolesAsync(string email);
		Task AddAsync(User user);
		Task UpdateAsync(User user);
		Task SaveChangesAsync();
		Task<bool> EmailExistsAsync(string email);
	}
}