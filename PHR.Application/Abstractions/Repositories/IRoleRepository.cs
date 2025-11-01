using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PHR.Domain.Entities;
namespace PHR.Application.Abstractions.Repositories
{
	public interface IRoleRepository
	{
		Task<IReadOnlyList<Guid>> GetRoleIdsByNamesAsync(IEnumerable<string> roleNames);
		Task AssignRolesAsync(Guid userId, IEnumerable<Guid> roleIds);
		Task<Role?> GetByNameAsync(string name);
		Task<Role?> GetByIdAsync(Guid id);
	}
}