using System;
namespace PHR.Domain.Entities
{
	public class RefreshToken
	{
		public Guid Id { get; set; }
		public string Token { get; set; } = string.Empty;
		public Guid UserId { get; set; }
		public DateTime ExpiryDateUtc { get; set; }
		public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
		public bool IsRevoked { get; set; }
		public string? RevokedReason { get; set; }
		public User User { get; set; } = null!;
		public bool IsExpired => DateTime.UtcNow >= ExpiryDateUtc;
		public bool IsActive => !IsRevoked && !IsExpired;
	}
}