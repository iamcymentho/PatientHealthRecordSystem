using Microsoft.AspNetCore.Authorization;
using PHR.Domain.Constants;
namespace PHR.Infrastructure.Authorization
{
	public static class AuthorizationPolicies
	{
		public const string ViewPatientRecords = "ViewPatientRecords";
		public const string CreatePatientRecords = "CreatePatientRecords";
		public const string ApproveAccessRequests = "ApproveAccessRequests";
		public const string ManageUsers = "ManageUsers";
		public const string ManageRoles = "ManageRoles";
		public static void AddPolicies(AuthorizationOptions options)
		{
			options.AddPolicy(ViewPatientRecords, policy =>
				policy.Requirements.Add(new PermissionRequirement(Permissions.ViewPatientRecords)));
			options.AddPolicy(CreatePatientRecords, policy =>
				policy.Requirements.Add(new PermissionRequirement(Permissions.CreatePatientRecords)));
			options.AddPolicy(ApproveAccessRequests, policy =>
				policy.Requirements.Add(new PermissionRequirement(Permissions.ApproveAccessRequests)));
			options.AddPolicy(ManageUsers, policy =>
				policy.Requirements.Add(new PermissionRequirement(Permissions.ManageUsers)));
			options.AddPolicy(ManageRoles, policy =>
				policy.Requirements.Add(new PermissionRequirement(Permissions.ManageRoles)));
		}
	}
}