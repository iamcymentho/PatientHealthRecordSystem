using System;
using PHR.Domain.Entities.Enums;
namespace PHR.Domain.Entities
{
	public class AccessRequest
	{
		public Guid Id { get; set; }
		public Guid PatientRecordId { get; set; }
		public Guid RequestorUserId { get; set; }
		public string Reason { get; set; } = string.Empty;
		public DateTime RequestDateUtc { get; set; } = DateTime.UtcNow;
		public AccessRequestStatus Status { get; set; } = AccessRequestStatus.Pending;
		public Guid? ApprovedByUserId { get; set; }
		public DateTime? ApprovedStartUtc { get; set; }
		public DateTime? ApprovedEndUtc { get; set; }
		public DateTime? DecisionDateUtc { get; set; }
		public string? DeclineReason { get; set; }
		// Navigation properties
		public PatientRecord? PatientRecord { get; set; }
		public User? RequestorUser { get; set; }
		public User? ApprovedByUser { get; set; }
	}
}