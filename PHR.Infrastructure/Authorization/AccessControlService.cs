using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PHR.Domain.Entities.Enums;
namespace PHR.Infrastructure.Authorization
{
	public class AccessControlService : IAccessControlService
	{
		private readonly PHRDbContext _context;
		public AccessControlService(PHRDbContext context)
		{
			_context = context;
		}
		public async Task<bool> HasPermissionAsync(Guid userId, string permission)
		{
			return await _context.Users
				.Where(u => u.Id == userId && u.IsActive)
				.SelectMany(u => u.UserRoles)
				.SelectMany(ur => ur.Role.RolePermissions)
				.AnyAsync(rp => rp.Permission.Name == permission);
		}
		public async Task<bool> CanAccessPatientRecordAsync(Guid userId, Guid patientRecordId)
		{
			var record = await _context.PatientRecords
				.FirstOrDefaultAsync(pr => pr.Id == patientRecordId);
			if (record == null)
				return false;
			// User can access if they created the record
			if (record.CreatedByUserId == userId)
				return true;
			// Check if user has viewPatientRecords permission (can see all records)
			var hasViewAllPermission = await HasPermissionAsync(userId, Domain.Constants.Permissions.ViewPatientRecords);
			if (hasViewAllPermission)
				return true;
			// Check for approved access requests within valid time range
			var hasApprovedAccess = await _context.AccessRequests
				.AnyAsync(ar => ar.PatientRecordId == patientRecordId &&
				               ar.RequestorUserId == userId &&
				               ar.Status == AccessRequestStatus.Approved &&
				               ar.ApprovedStartUtc <= DateTime.UtcNow &&
				               ar.ApprovedEndUtc >= DateTime.UtcNow);
			return hasApprovedAccess;
		}
		public async Task<bool> IsRecordOwnerAsync(Guid userId, Guid patientRecordId)
		{
			return await _context.PatientRecords
				.AnyAsync(pr => pr.Id == patientRecordId && pr.CreatedByUserId == userId);
		}
	}
}