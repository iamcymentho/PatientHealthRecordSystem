using Microsoft.EntityFrameworkCore;
using PHR.Application.Abstractions.Data;
using PHR.Domain.Entities;
namespace PHR.Infrastructure
{
	public class PHRDbContext : DbContext, IApplicationDbContext
	{
		public PHRDbContext(DbContextOptions<PHRDbContext> options) : base(options)
		{
		}
	public DbSet<User> Users => Set<User>();
	public DbSet<Role> Roles => Set<Role>();
	public DbSet<Permission> Permissions => Set<Permission>();
	public DbSet<UserRole> UserRoles => Set<UserRole>();
	public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
	public DbSet<PatientRecord> PatientRecords => Set<PatientRecord>();
	public DbSet<AccessRequest> AccessRequests => Set<AccessRequest>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
	public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
			modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
			modelBuilder.Entity<PatientRecord>().HasQueryFilter(pr => !pr.IsDeleted);
			// AccessRequest -> PatientRecord relationship
			modelBuilder.Entity<AccessRequest>()
				.HasOne(ar => ar.PatientRecord)
				.WithMany(pr => pr.AccessRequests)
				.HasForeignKey(ar => ar.PatientRecordId)
				.OnDelete(DeleteBehavior.Restrict);
			// RefreshToken configuration
			modelBuilder.Entity<RefreshToken>()
				.HasOne(rt => rt.User)
				.WithMany()
				.HasForeignKey(rt => rt.UserId)
				.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<RefreshToken>()
				.HasIndex(rt => rt.Token)
				.IsUnique();
			// PasswordResetToken configuration
			modelBuilder.Entity<PasswordResetToken>()
				.HasOne(prt => prt.User)
				.WithMany()
				.HasForeignKey(prt => prt.UserId)
				.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<PasswordResetToken>()
				.HasIndex(prt => prt.Token)
				.IsUnique();
			modelBuilder.Entity<PasswordResetToken>()
				.HasIndex(prt => new { prt.UserId, prt.ExpiryDateUtc, prt.IsUsed });
			// Performance Indexes
			// User indexes
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();
			// PatientRecord indexes for common queries
			modelBuilder.Entity<PatientRecord>()
				.HasIndex(pr => pr.PatientName);
			modelBuilder.Entity<PatientRecord>()
				.HasIndex(pr => pr.CreatedByUserId);
			modelBuilder.Entity<PatientRecord>()
				.HasIndex(pr => pr.CreatedDateUtc);
			modelBuilder.Entity<PatientRecord>()
				.HasIndex(pr => pr.Diagnosis);
			// Composite index for common filter combinations
			modelBuilder.Entity<PatientRecord>()
				.HasIndex(pr => new { pr.CreatedByUserId, pr.CreatedDateUtc, pr.IsDeleted });
			// AccessRequest indexes
			modelBuilder.Entity<AccessRequest>()
				.HasIndex(ar => ar.RequestorUserId);
			modelBuilder.Entity<AccessRequest>()
				.HasIndex(ar => ar.Status);
			modelBuilder.Entity<AccessRequest>()
				.HasIndex(ar => new { ar.RequestorUserId, ar.Status });
			modelBuilder.Entity<AccessRequest>()
				.HasIndex(ar => new { ar.PatientRecordId, ar.Status });
			modelBuilder.Entity<AccessRequest>()
				.HasIndex(ar => ar.RequestDateUtc);
			// AuditLog configuration
			modelBuilder.Entity<AuditLog>()
				.HasOne(al => al.User)
				.WithMany()
				.HasForeignKey(al => al.UserId)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<AuditLog>()
				.HasIndex(al => al.TimestampUtc);
			modelBuilder.Entity<AuditLog>()
				.HasIndex(al => new { al.UserId, al.TimestampUtc });
			modelBuilder.Entity<AuditLog>()
				.HasIndex(al => al.Action);
		}
	}
}