using System;
using System.Collections.Generic;
namespace PHR.Domain.Entities
{
	public class PatientRecord
	{
		public Guid Id { get; set; }
		public string PatientName { get; set; } = string.Empty;
		public DateTime DateOfBirth { get; set; }
		public string Diagnosis { get; set; } = string.Empty;
		public string TreatmentPlan { get; set; } = string.Empty;
		public string MedicalHistory { get; set; } = string.Empty;
		public string Medications { get; set; } = string.Empty;
		public string Allergies { get; set; } = string.Empty;
		public string VitalSigns { get; set; } = string.Empty;
		public string LabResults { get; set; } = string.Empty;
		public string Imaging { get; set; } = string.Empty;
		public Guid CreatedByUserId { get; set; }
		public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;
		public Guid? LastModifiedByUserId { get; set; }
		public DateTime? LastModifiedDateUtc { get; set; }
		public bool IsDeleted { get; set; }
		// Navigation properties
		public User? CreatedByUser { get; set; }
		public User? LastModifiedByUser { get; set; }
		public ICollection<AccessRequest> AccessRequests { get; set; } = new List<AccessRequest>();
	}
}