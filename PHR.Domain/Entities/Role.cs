using System;
using System.Collections.Generic;
namespace PHR.Domain.Entities
{
	public class Role
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
		public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
	}
}