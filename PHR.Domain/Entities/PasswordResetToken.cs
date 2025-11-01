using System;
namespace PHR.Domain.Entities
{
	public class PasswordResetToken
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string Token { get; set; } = string.Empty;
		public DateTime ExpiryDateUtc { get; set; }
		public bool IsUsed { get; set; } = false;
		public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
		public DateTime? UsedAtUtc { get; set; }
		public User User { get; set; } = null!;
	}
}