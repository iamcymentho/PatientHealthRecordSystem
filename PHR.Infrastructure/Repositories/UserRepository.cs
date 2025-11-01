using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Repositories;
using PHR.Domain.Entities;
namespace PHR.Infrastructure.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly PHRDbContext _db;
		public UserRepository(PHRDbContext db)
		{
			_db = db;
		}
		public Task<User?> GetByIdAsync(Guid id)
		{
			return _db.Users.FirstOrDefaultAsync(u => u.Id == id);
		}
		public Task<User?> GetByEmailAsync(string email)
		{
			return _db.Users.FirstOrDefaultAsync(u => u.Email == email);
		}
		public Task<User?> GetByEmailWithRolesAsync(string email)
		{
			return _db.Users
				.AsNoTracking()
				.Include(u => u.UserRoles)
					.ThenInclude(ur => ur.Role)
						.ThenInclude(r => r.RolePermissions)
							.ThenInclude(rp => rp.Permission)
				.FirstOrDefaultAsync(u => u.Email == email);
		}
		public async Task AddAsync(User user)
		{
			await _db.Users.AddAsync(user);
			await _db.SaveChangesAsync();
		}
		public Task UpdateAsync(User user)
		{
			_db.Users.Update(user);
			return Task.CompletedTask;
		}
		public Task SaveChangesAsync()
		{
			return _db.SaveChangesAsync();
		}
		public Task<bool> EmailExistsAsync(string email)
		{
			return _db.Users.AnyAsync(u => u.Email == email);
		}
	}
}