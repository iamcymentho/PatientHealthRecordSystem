using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PHR.Application.Abstractions.Services;
using PHR.Infrastructure.Configuration;
namespace PHR.Infrastructure.Services
{
	public class EmailService : IEmailService
	{
		private readonly ILogger<EmailService> _logger;
		private readonly SmtpSettings _smtpSettings;
		public EmailService(ILogger<EmailService> logger, IOptions<SmtpSettings> smtpSettings)
		{
			_logger = logger;
			_smtpSettings = smtpSettings.Value;
		}
		public async Task SendAccessRequestApprovedAsync(
			string recipientEmail,
			string recipientName,
			string patientName,
			string approvedByName,
			string startDate,
			string endDate)
		{
			var subject = "Access Request Approved - PHR System";
			var body = $@"
Dear {recipientName},
Your access request for patient {patientName} has been approved by {approvedByName}.
Access Details:
- Patient: {patientName}
- Approved by: {approvedByName}
- Access period: {startDate} to {endDate}
You can now access the patient's records within the specified time period.
Best regards,
PHR System Team
";
			await SendEmailAsync(recipientEmail, subject, body);
		}
		public async Task SendAccessRequestDeclinedAsync(
			string recipientEmail,
			string recipientName,
			string patientName,
			string declinedByName,
			string reason)
		{
			var subject = "Access Request Declined - PHR System";
			var body = $@"
Dear {recipientName},
Your access request for patient {patientName} has been declined by {declinedByName}.
Details:
- Patient: {patientName}
- Declined by: {declinedByName}
- Reason: {reason}
If you believe this decision was made in error, please contact the administrator.
Best regards,
PHR System Team
";
			await SendEmailAsync(recipientEmail, subject, body);
		}
		public async Task SendAccessRequestCreatedAsync(
			string recipientEmail,
			string recipientName,
			string requestorName,
			string patientName,
			string reason)
		{
			var subject = "New Access Request - PHR System";
			var body = $@"
Dear {recipientName},
A new access request has been submitted for your review.
Request Details:
- Requestor: {requestorName}
- Patient: {patientName}
- Reason: {reason}
Please log into the PHR system to review and approve/decline this request.
Best regards,
PHR System Team
";
			await SendEmailAsync(recipientEmail, subject, body);
		}
		public async Task SendWelcomeEmailAsync(string recipientEmail, string recipientName)
		{
			var subject = "Welcome to PHR System";
			var body = $@"
Dear {recipientName},
Welcome to the Patient Health Records (PHR) System!
Your account has been successfully created. You can now:
- Access patient records based on your permissions
- Submit access requests for additional patient records
- Manage your profile and settings
Please keep your login credentials secure and report any suspicious activity.
Best regards,
PHR System Team
";
			await SendEmailAsync(recipientEmail, subject, body);
		}
		private async Task SendEmailAsync(string recipientEmail, string subject, string body)
		{
			try
			{
				_logger.LogInformation("Sending email to {Email} with subject: {Subject}", recipientEmail, subject);
				if (string.IsNullOrEmpty(_smtpSettings.Host) || string.IsNullOrEmpty(_smtpSettings.Username))
				{
					_logger.LogWarning("SMTP not configured. Email will be logged only.");
					_logger.LogInformation("Email content - To: {Email}, Subject: {Subject}, Body: {Body}", 
						recipientEmail, subject, body);
					return;
				}
				var message = new MimeMessage();
				message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
				message.To.Add(new MailboxAddress("", recipientEmail));
				message.Subject = subject;
				var bodyBuilder = new BodyBuilder
				{
					TextBody = body,
					HtmlBody = $"<pre>{body}</pre>"
				};
				message.Body = bodyBuilder.ToMessageBody();
				using var client = new SmtpClient();
				await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, _smtpSettings.EnableSsl);
				await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);
				_logger.LogInformation("Email sent successfully to {Email}", recipientEmail);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to send email to {Email}. Error: {Error}", recipientEmail, ex.Message);
				_logger.LogInformation("Failed email content - To: {Email}, Subject: {Subject}, Body: {Body}", 
					recipientEmail, subject, body);
			}
		}
	}
}