using System.Threading.Tasks;
namespace PHR.Application.Abstractions.Services
{
	public interface IEmailService
	{
		Task SendAccessRequestApprovedAsync(string recipientEmail, string recipientName, string patientName, string approvedByName, string startDate, string endDate);
		Task SendAccessRequestDeclinedAsync(string recipientEmail, string recipientName, string patientName, string declinedByName, string reason);
		Task SendAccessRequestCreatedAsync(string recipientEmail, string recipientName, string requestorName, string patientName, string reason);
		Task SendWelcomeEmailAsync(string recipientEmail, string recipientName);
	}
	public class EmailTemplate
	{
		public string Subject { get; set; } = string.Empty;
		public string Body { get; set; } = string.Empty;
	}
}