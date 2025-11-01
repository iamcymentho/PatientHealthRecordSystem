using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PHR.Domain.Constants;
using PHR.Domain.Entities;
using PHR.Domain.Entities.Enums;
using PHR.Infrastructure.Security;
namespace PHR.Infrastructure.Data
{
	public static class DbSeeder
	{
		public static async Task SeedAsync(IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<PHRDbContext>();
			var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<PHRDbContext>>();
			try
			{
				await context.Database.MigrateAsync();
				await SeedPermissionsAsync(context, logger);
				await SeedRolesAsync(context, logger);
				await SeedUsersAsync(context, passwordHasher, logger);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while seeding the database");
				throw;
			}
		}
		private static async Task SeedPermissionsAsync(PHRDbContext context, ILogger logger)
		{
			if (await context.Permissions.AnyAsync())
			{
				logger.LogInformation("Permissions already exist, skipping seeding");
				return;
			}
			var permissions = new List<Permission>();
			foreach (var permissionName in Permissions.All)
			{
				permissions.Add(new Permission
				{
					Id = Guid.NewGuid(),
					Name = permissionName,
					Description = GetPermissionDescription(permissionName)
				});
			}
			await context.Permissions.AddRangeAsync(permissions);
			await context.SaveChangesAsync();
			logger.LogInformation("Seeded {Count} permissions", permissions.Count);
		}
		private static async Task SeedRolesAsync(PHRDbContext context, ILogger logger)
		{
			if (await context.Roles.AnyAsync())
			{
				logger.LogInformation("Roles already exist, skipping seeding");
				return;
			}
			var permissions = await context.Permissions.ToListAsync();
			var roles = new List<Role>();
			// Admin Role - has all permissions
			var adminRole = new Role
			{
				Id = Guid.NewGuid(),
				Name = Domain.Constants.Roles.Admin
			};
			roles.Add(adminRole);
			// Doctor Role - can view, create, and approve access requests
			var doctorRole = new Role
			{
				Id = Guid.NewGuid(),
				Name = Domain.Constants.Roles.Doctor
			};
			roles.Add(doctorRole);
			// Nurse Role - can create records and request access
			var nurseRole = new Role
			{
				Id = Guid.NewGuid(),
				Name = Domain.Constants.Roles.Nurse
			};
			roles.Add(nurseRole);
			// Patient Role - basic access only
			var patientRole = new Role
			{
				Id = Guid.NewGuid(),
				Name = Domain.Constants.Roles.Patient
			};
			roles.Add(patientRole);
			await context.Roles.AddRangeAsync(roles);
			await context.SaveChangesAsync();
			// Assign permissions to roles
			var rolePermissions = new List<RolePermission>();
			// Admin gets all permissions
			foreach (var permission in permissions)
			{
				rolePermissions.Add(new RolePermission
				{
					RoleId = adminRole.Id,
					PermissionId = permission.Id
				});
			}
			// Doctor gets view, create, and approve permissions
			var doctorPermissions = permissions.Where(p => 
				p.Name == Permissions.ViewPatientRecords ||
				p.Name == Permissions.CreatePatientRecords ||
				p.Name == Permissions.ApproveAccessRequests).ToList();
			foreach (var permission in doctorPermissions)
			{
				rolePermissions.Add(new RolePermission
				{
					RoleId = doctorRole.Id,
					PermissionId = permission.Id
				});
			}
			// Nurse gets create permission
			var nursePermissions = permissions.Where(p => 
				p.Name == Permissions.CreatePatientRecords).ToList();
			foreach (var permission in nursePermissions)
			{
				rolePermissions.Add(new RolePermission
				{
					RoleId = nurseRole.Id,
					PermissionId = permission.Id
				});
			}
			// Patient gets no specific permissions (they can only view their own records via business logic)
			await context.RolePermissions.AddRangeAsync(rolePermissions);
			await context.SaveChangesAsync();
			logger.LogInformation("Seeded {Count} roles with permissions", roles.Count);
		}
		private static async Task SeedUsersAsync(PHRDbContext context, IPasswordHasher passwordHasher, ILogger logger)
		{
			if (await context.Users.AnyAsync())
			{
				logger.LogInformation("Users already exist, skipping seeding");
				return;
			}
			var roles = await context.Roles.ToListAsync();
			var users = new List<User>();
			// Admin user
			var adminUser = new User
			{
				Id = Guid.NewGuid(),
				FullName = "System Administrator",
				Email = "admin@phr.com",
				PasswordHash = passwordHasher.Hash("Admin123!"),
				Gender = Gender.Male,
				PhoneNumber = "+1234567890",
				IsActive = true,
				CreatedAtUtc = DateTime.UtcNow
			};
			users.Add(adminUser);
			// Doctor user
			var doctorUser = new User
			{
				Id = Guid.NewGuid(),
				FullName = "Dr. John Smith",
				Email = "doctor@phr.com",
				PasswordHash = passwordHasher.Hash("Doctor123!"),
				Gender = Gender.Male,
				PhoneNumber = "+1234567891",
				IsActive = true,
				CreatedAtUtc = DateTime.UtcNow
			};
			users.Add(doctorUser);
			// Nurse user
			var nurseUser = new User
			{
				Id = Guid.NewGuid(),
				FullName = "Jane Doe",
				Email = "nurse@phr.com",
				PasswordHash = passwordHasher.Hash("Nurse123!"),
				Gender = Gender.Female,
				PhoneNumber = "+1234567892",
				IsActive = true,
				CreatedAtUtc = DateTime.UtcNow
			};
			users.Add(nurseUser);
			// Patient user
			var patientUser = new User
			{
				Id = Guid.NewGuid(),
				FullName = "Bob Johnson",
				Email = "patient@phr.com",
				PasswordHash = passwordHasher.Hash("Patient123!"),
				Gender = Gender.Male,
				PhoneNumber = "+1234567893",
				IsActive = true,
				CreatedAtUtc = DateTime.UtcNow
			};
			users.Add(patientUser);
			await context.Users.AddRangeAsync(users);
			await context.SaveChangesAsync();
			// Assign roles to users
			var userRoles = new List<UserRole>
			{
				new() { UserId = adminUser.Id, RoleId = roles.First(r => r.Name == Domain.Constants.Roles.Admin).Id },
				new() { UserId = doctorUser.Id, RoleId = roles.First(r => r.Name == Domain.Constants.Roles.Doctor).Id },
				new() { UserId = nurseUser.Id, RoleId = roles.First(r => r.Name == Domain.Constants.Roles.Nurse).Id },
				new() { UserId = patientUser.Id, RoleId = roles.First(r => r.Name == Domain.Constants.Roles.Patient).Id }
			};
			await context.UserRoles.AddRangeAsync(userRoles);
			await context.SaveChangesAsync();
			logger.LogInformation("Seeded {Count} users with roles", users.Count);
		}
		private static string GetPermissionDescription(string permissionName)
		{
			return permissionName switch
			{
				Permissions.ViewPatientRecords => "Can view all patient health records",
				Permissions.CreatePatientRecords => "Can create new patient health records",
				Permissions.ApproveAccessRequests => "Can approve or decline access requests for patient records",
				Permissions.ManageUsers => "Can create, update, and manage user accounts",
				Permissions.ManageRoles => "Can create and manage roles and permissions",
				_ => $"Permission: {permissionName}"
			};
		}
	}
}