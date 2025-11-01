namespace PHR.Domain.Constants
{
	public static class Permissions
	{
		public const string ViewPatientRecords = "viewPatientRecords";
		public const string CreatePatientRecords = "createPatientRecords";
		public const string ApproveAccessRequests = "approveAccessRequests";
		public const string ManageUsers = "manageUsers";
		public const string ManageRoles = "manageRoles";
		public const string ViewAuditLogs = "viewAuditLogs";
		public static readonly string[] All = 
		{
			ViewPatientRecords,
			CreatePatientRecords,
			ApproveAccessRequests,
			ManageUsers,
			ManageRoles,
			ViewAuditLogs
		};
	}
	public static class Roles
	{
		public const string Admin = "Admin";
		public const string Doctor = "Doctor";
		public const string Nurse = "Nurse";
		public const string Patient = "Patient";
		public static readonly string[] All = 
		{
			Admin,
			Doctor,
			Nurse,
			Patient
		};
	}
}