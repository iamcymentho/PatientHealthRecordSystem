using System;
using System.Threading.Tasks;
using PHR.Application.Abstractions.Repositories;
using PHR.Domain.Entities;
namespace PHR.Infrastructure.Repositories
{
	public class UserRoleRepository : IUserRoleRepository
	{
		private readonly PHRDbContext _context;
		public UserRoleRepository(PHRDbContext context)
		{
			_context = context;
		}
		public async Task AddAsync(UserRole userRole)
		{
			await _context.UserRoles.AddAsync(userRole);
		}
		public async Task AddRangeAsync(UserRole[] userRoles)
		{
			await _context.UserRoles.AddRangeAsync(userRoles);
		}
		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}