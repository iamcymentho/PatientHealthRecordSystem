using System;
using System.Threading.Tasks;
using PHR.Application.Abstractions.Data;
using PHR.Application.Abstractions.Services;
using PHR.Domain.Entities;
namespace PHR.Infrastructure.Services
{
	public class AuditService : IAuditService
	{
		private readonly IApplicationDbContext _context;
		public AuditService(IApplicationDbContext context)
		{
			_context = context;
		}
		public async Task LogAsync(
			Guid userId,
			string action,
			string entityType,
			Guid? entityId = null,
			string details = "",
			string ipAddress = "",
			string userAgent = "")
		{
			var auditLog = new AuditLog
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				Action = action,
				EntityType = entityType,
				EntityId = entityId,
				Details = details,
				TimestampUtc = DateTime.UtcNow,
				IpAddress = ipAddress,
				UserAgent = userAgent
			};
			_context.AuditLogs.Add(auditLog);
			await _context.SaveChangesAsync();
		}
		public async Task LogPatientRecordAccessAsync(
			Guid userId,
			Guid patientRecordId,
			string patientName,
			string ipAddress = "",
			string userAgent = "")
		{
			await LogAsync(
				userId,
				AuditActions.PatientRecordViewed,
				"PatientRecord",
				patientRecordId,
				$"Accessed patient record for: {patientName}",
				ipAddress,
				userAgent);
		}
		public async Task LogAccessRequestActionAsync(
			Guid userId,
			Guid accessRequestId,
			string action,
			string details = "",
			string ipAddress = "",
			string userAgent = "")
		{
			await LogAsync(
				userId,
				action,
				"AccessRequest",
				accessRequestId,
				details,
				ipAddress,
				userAgent);
		}
		public async Task LogAuthenticationAsync(
			Guid userId,
			string action,
			string details = "",
			string ipAddress = "",
			string userAgent = "")
		{
			await LogAsync(
				userId,
				action,
				"User",
				userId,
				details,
				ipAddress,
				userAgent);
		}
	}
}