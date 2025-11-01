using Microsoft.AspNetCore.Authorization;
namespace PHR.Infrastructure.Authorization
{
	public class PermissionRequirement : IAuthorizationRequirement
	{
		public string Permission { get; }
		public PermissionRequirement(string permission)
		{
			Permission = permission;
		}
	}
}