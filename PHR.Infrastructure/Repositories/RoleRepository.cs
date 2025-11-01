using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Repositories;
using PHR.Domain.Entities;
namespace PHR.Infrastructure.Repositories
{
	public class RoleRepository : IRoleRepository
	{
		private readonly PHRDbContext _db;
		public RoleRepository(PHRDbContext db) { _db = db; }
		public async Task<IReadOnlyList<Guid>> GetRoleIdsByNamesAsync(IEnumerable<string> roleNames)
		{
			var names = roleNames.Select(r => r.Trim()).Where(r => r != string.Empty).ToArray();
			return await _db.Roles.Where(r => names.Contains(r.Name)).Select(r => r.Id).ToListAsync();
		}
		public async Task AssignRolesAsync(Guid userId, IEnumerable<Guid> roleIds)
		{
			var existing = await _db.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
			_db.UserRoles.RemoveRange(existing);
			var add = roleIds.Select(rid => new UserRole { UserId = userId, RoleId = rid });
			await _db.UserRoles.AddRangeAsync(add);
			await _db.SaveChangesAsync();
		}
		public async Task<Role?> GetByNameAsync(string name)
		{
			return await _db.Roles.FirstOrDefaultAsync(r => r.Name == name);
		}
		public async Task<Role?> GetByIdAsync(Guid id)
		{
			return await _db.Roles.FirstOrDefaultAsync(r => r.Id == id);
		}
	}
}