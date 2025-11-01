using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PHR.Domain.Entities;
namespace PHR.Application.Abstractions.Data
{
	public interface IApplicationDbContext
	{
		DbSet<User> Users { get; }
		DbSet<Role> Roles { get; }
		DbSet<Permission> Permissions { get; }
		DbSet<UserRole> UserRoles { get; }
		DbSet<RolePermission> RolePermissions { get; }
		DbSet<PatientRecord> PatientRecords { get; }
		DbSet<AccessRequest> AccessRequests { get; }
		DbSet<RefreshToken> RefreshTokens { get; }
		DbSet<AuditLog> AuditLogs { get; }
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}