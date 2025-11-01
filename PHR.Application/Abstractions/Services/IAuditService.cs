using System;
using System.Threading.Tasks;
namespace PHR.Application.Abstractions.Services
{
	public interface IAuditService
	{
		Task LogAsync(
			Guid userId,
			string action,
			string entityType,
			Guid? entityId = null,
			string details = "",
			string ipAddress = "",
			string userAgent = "");
		Task LogPatientRecordAccessAsync(Guid userId, Guid patientRecordId, string patientName, string ipAddress = "", string userAgent = "");
		Task LogAccessRequestActionAsync(Guid userId, Guid accessRequestId, string action, string details = "", string ipAddress = "", string userAgent = "");
		Task LogAuthenticationAsync(Guid userId, string action, string details = "", string ipAddress = "", string userAgent = "");
	}
}