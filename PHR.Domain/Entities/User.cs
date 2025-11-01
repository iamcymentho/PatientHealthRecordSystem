using System;
using System.Collections.Generic;
using PHR.Domain.Entities.Enums;
namespace PHR.Domain.Entities
{
	public class User
	{
		public Guid Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public Gender Gender { get; set; } = Gender.Male;
		public string? PhoneNumber { get; set; }
		public bool IsActive { get; set; } = true;
		public bool RequirePasswordChange { get; set; } = false;
		public DateTime? LastPasswordChangeUtc { get; set; }
		public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
		public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
	}
}