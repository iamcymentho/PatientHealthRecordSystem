using System;
using System.Threading.Tasks;
namespace PHR.Infrastructure.Authorization
{
	public interface IAccessControlService
	{
		Task<bool> HasPermissionAsync(Guid userId, string permission);
		Task<bool> CanAccessPatientRecordAsync(Guid userId, Guid patientRecordId);
		Task<bool> IsRecordOwnerAsync(Guid userId, Guid patientRecordId);
	}
}