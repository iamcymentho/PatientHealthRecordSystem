using System;
using PHR.Domain.Entities.Enums;
namespace PHR.Domain.Entities
{
	public class AuditLog
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string Action { get; set; } = string.Empty;
		public string EntityType { get; set; } = string.Empty;
		public Guid? EntityId { get; set; }
		public string Details { get; set; } = string.Empty;
		public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
		public string IpAddress { get; set; } = string.Empty;
		public string UserAgent { get; set; } = string.Empty;
		// Navigation properties
		public User? User { get; set; }
	}
	public static class AuditActions
	{
		public const string PatientRecordViewed = "PatientRecord.Viewed";
		public const string PatientRecordCreated = "PatientRecord.Created";
		public const string PatientRecordUpdated = "PatientRecord.Updated";
		public const string PatientRecordDeleted = "PatientRecord.Deleted";
		public const string AccessRequestCreated = "AccessRequest.Created";
		public const string AccessRequestApproved = "AccessRequest.Approved";
		public const string AccessRequestDeclined = "AccessRequest.Declined";
		public const string UserLoggedIn = "User.LoggedIn";
		public const string UserLoggedOut = "User.LoggedOut";
		public const string UserRegistered = "User.Registered";
	}
}