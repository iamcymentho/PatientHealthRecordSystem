using System;
using System.Threading.Tasks;
using PHR.Domain.Entities;
namespace PHR.Application.Abstractions.Repositories
{
	public interface IUserRoleRepository
	{
		Task AddAsync(UserRole userRole);
		Task AddRangeAsync(UserRole[] userRoles);
		Task SaveChangesAsync();
	}
}