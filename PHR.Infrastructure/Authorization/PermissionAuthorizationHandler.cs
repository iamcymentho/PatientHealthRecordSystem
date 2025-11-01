using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PHR.Infrastructure;
namespace PHR.Infrastructure.Authorization
{
	public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
	{
		private readonly PHRDbContext _context;
		public PermissionAuthorizationHandler(PHRDbContext context)
		{
			_context = context;
		}
		protected override async Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			PermissionRequirement requirement)
		{
			var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
			{
				return;
			}
			// Check if user has the required permission through their roles
			var hasPermission = await _context.Users
				.Where(u => u.Id == userId && u.IsActive)
				.SelectMany(u => u.UserRoles)
				.SelectMany(ur => ur.Role.RolePermissions)
				.AnyAsync(rp => rp.Permission.Name == requirement.Permission);
			if (hasPermission)
			{
				context.Succeed(requirement);
			}
		}
	}
}