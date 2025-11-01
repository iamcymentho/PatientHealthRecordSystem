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
			var content = $@"
				<p>Dear <strong>{recipientName}</strong>,</p>
				<p>Great news! Your access request for patient <strong>{patientName}</strong> has been approved by <strong>{approvedByName}</strong>.</p>

				<div style=""background-color: #f7fafc; border-left: 4px solid #48bb78; padding: 20px; margin: 20px 0; border-radius: 4px;"">
					<h3 style=""margin: 0 0 15px 0; color: #2d3748; font-size: 18px;"">📋 Access Details</h3>
					<table style=""width: 100%; color: #4a5568; font-size: 15px;"">
						<tr>
							<td style=""padding: 8px 0;""><strong>Patient:</strong></td>
							<td style=""padding: 8px 0;"">{patientName}</td>
						</tr>
						<tr>
							<td style=""padding: 8px 0;""><strong>Approved by:</strong></td>
							<td style=""padding: 8px 0;"">{approvedByName}</td>
						</tr>
						<tr>
							<td style=""padding: 8px 0;""><strong>Access period:</strong></td>
							<td style=""padding: 8px 0;"">{startDate} to {endDate}</td>
						</tr>
					</table>
				</div>

				<p>You can now access the patient's records within the specified time period.</p>
				<p style=""margin-top: 30px;"">Best regards,<br><strong>PHR System Team</strong></p>
			";
			var htmlBody = CreateHtmlTemplate("Access Request Approved ✅", content);
			await SendEmailAsync(recipientEmail, subject, htmlBody);
		}
		public async Task SendAccessRequestDeclinedAsync(
			string recipientEmail,
			string recipientName,
			string patientName,
			string declinedByName,
			string reason)
		{
			var subject = "Access Request Declined - PHR System";
			var content = $@"
				<p>Dear <strong>{recipientName}</strong>,</p>
				<p>Your access request for patient <strong>{patientName}</strong> has been declined by <strong>{declinedByName}</strong>.</p>

				<div style=""background-color: #fff5f5; border-left: 4px solid #f56565; padding: 20px; margin: 20px 0; border-radius: 4px;"">
					<h3 style=""margin: 0 0 15px 0; color: #2d3748; font-size: 18px;"">📋 Details</h3>
					<table style=""width: 100%; color: #4a5568; font-size: 15px;"">
						<tr>
							<td style=""padding: 8px 0;""><strong>Patient:</strong></td>
							<td style=""padding: 8px 0;"">{patientName}</td>
						</tr>
						<tr>
							<td style=""padding: 8px 0;""><strong>Declined by:</strong></td>
							<td style=""padding: 8px 0;"">{declinedByName}</td>
						</tr>
						<tr>
							<td style=""padding: 8px 0;""><strong>Reason:</strong></td>
							<td style=""padding: 8px 0;"">{reason}</td>
						</tr>
					</table>
				</div>

				<p>If you believe this decision was made in error, please contact the administrator.</p>
				<p style=""margin-top: 30px;"">Best regards,<br><strong>PHR System Team</strong></p>
			";
			var htmlBody = CreateHtmlTemplate("Access Request Declined", content);
			await SendEmailAsync(recipientEmail, subject, htmlBody);
		}
		public async Task SendAccessRequestCreatedAsync(
			string recipientEmail,
			string recipientName,
			string requestorName,
			string patientName,
			string reason)
		{
			var subject = "New Access Request - PHR System";
			var content = $@"
				<p>Dear <strong>{recipientName}</strong>,</p>
				<p>A new access request has been submitted for your review.</p>

				<div style=""background-color: #fffaf0; border-left: 4px solid #ed8936; padding: 20px; margin: 20px 0; border-radius: 4px;"">
					<h3 style=""margin: 0 0 15px 0; color: #2d3748; font-size: 18px;"">📋 Request Details</h3>
					<table style=""width: 100%; color: #4a5568; font-size: 15px;"">
						<tr>
							<td style=""padding: 8px 0;""><strong>Requestor:</strong></td>
							<td style=""padding: 8px 0;"">{requestorName}</td>
						</tr>
						<tr>
							<td style=""padding: 8px 0;""><strong>Patient:</strong></td>
							<td style=""padding: 8px 0;"">{patientName}</td>
						</tr>
						<tr>
							<td style=""padding: 8px 0;""><strong>Reason:</strong></td>
							<td style=""padding: 8px 0;"">{reason}</td>
						</tr>
					</table>
				</div>

				<p>Please log into the PHR system to review and approve/decline this request.</p>
				<p style=""margin-top: 30px;"">Best regards,<br><strong>PHR System Team</strong></p>
			";
			var htmlBody = CreateHtmlTemplate("New Access Request", content);
			await SendEmailAsync(recipientEmail, subject, htmlBody);
		}
		public async Task SendWelcomeEmailAsync(string recipientEmail, string recipientName)
		{
			var subject = "Welcome to PHR System";
			var content = $@"
				<p>Dear <strong>{recipientName}</strong>,</p>
				<p>Welcome to the <strong>Patient Health Records (PHR) System</strong>!</p>
				<p>Your account has been successfully created. You now have access to the following features:</p>

				<div style=""background-color: #f0fff4; border-left: 4px solid #48bb78; padding: 20px; margin: 20px 0; border-radius: 4px;"">
					<ul style=""margin: 0; padding-left: 20px; color: #2d3748; line-height: 2;"">
						<li>✅ Access patient records based on your permissions</li>
						<li>✅ Submit access requests for additional patient records</li>
						<li>✅ Manage your profile and settings</li>
					</ul>
				</div>

				<div style=""background-color: #fef3c7; border-left: 4px solid #f59e0b; padding: 15px; margin: 20px 0; border-radius: 4px;"">
					<p style=""margin: 0; color: #92400e;""><strong>🔒 Security Reminder:</strong> Please keep your login credentials secure and report any suspicious activity immediately.</p>
				</div>

				<p style=""margin-top: 30px;"">We're excited to have you on board!</p>
				<p>Best regards,<br><strong>PHR System Team</strong></p>
			";
			var htmlBody = CreateHtmlTemplate("Welcome to PHR System! 👋", content);
			await SendEmailAsync(recipientEmail, subject, htmlBody);
		}
		public async Task SendPasswordResetEmailAsync(string recipientEmail, string recipientName, string resetToken, DateTime expiryDate)
		{
			var subject = "Password Reset Request - PHR System";
			var content = $@"
				<p>Dear <strong>{recipientName}</strong>,</p>
				<p>We received a request to reset your password for your PHR System account.</p>

				<div style=""background-color: #eff6ff; border-left: 4px solid #3b82f6; padding: 20px; margin: 20px 0; border-radius: 4px;"">
					<h3 style=""margin: 0 0 15px 0; color: #1e40af; font-size: 18px;"">🔑 Password Reset Details</h3>
					<table style=""width: 100%; color: #1e3a8a; font-size: 15px;"">
						<tr>
							<td style=""padding: 8px 0; vertical-align: top;""><strong>Reset Token:</strong></td>
							<td style=""padding: 8px 0;"">
								<div style=""background-color: #dbeafe; padding: 12px; border-radius: 4px; font-family: 'Courier New', monospace; font-size: 20px; font-weight: bold; letter-spacing: 2px; color: #1e40af;"">{resetToken}</div>
							</td>
						</tr>
						<tr>
							<td style=""padding: 8px 0;""><strong>Expiry Time:</strong></td>
							<td style=""padding: 8px 0;"">{expiryDate:yyyy-MM-dd HH:mm:ss} UTC</td>
						</tr>
						<tr>
							<td style=""padding: 8px 0;""><strong>Valid for:</strong></td>
							<td style=""padding: 8px 0;"">1 hour from the time of request</td>
						</tr>
					</table>
				</div>

				<div style=""background-color: #f7fafc; padding: 20px; margin: 20px 0; border-radius: 4px; border: 1px solid #e2e8f0;"">
					<h3 style=""margin: 0 0 15px 0; color: #2d3748; font-size: 16px;"">📝 How to reset your password:</h3>
					<ol style=""margin: 0; padding-left: 20px; color: #4a5568; line-height: 2;"">
						<li>Copy the reset token provided above</li>
						<li>Navigate to the password reset endpoint in the PHR System</li>
						<li>Enter the token and your new password</li>
					</ol>
				</div>

				<div style=""background-color: #fef2f2; border-left: 4px solid #ef4444; padding: 15px; margin: 20px 0; border-radius: 4px;"">
					<p style=""margin: 0; color: #991b1b;""><strong>⚠️ Security Notice:</strong> If you did not request this password reset, please ignore this email and ensure your account is secure. This reset token will expire in 1 hour.</p>
				</div>

				<p style=""margin-top: 30px;"">Best regards,<br><strong>PHR System Team</strong></p>
			";
			var htmlBody = CreateHtmlTemplate("Password Reset Request 🔐", content);
			await SendEmailAsync(recipientEmail, subject, htmlBody);
		}
		public async Task SendPasswordChangedNotificationAsync(string recipientEmail, string recipientName)
		{
			var subject = "Password Changed Successfully - PHR System";
			var content = $@"
				<p>Dear <strong>{recipientName}</strong>,</p>
				<p>This is a confirmation that your password for your PHR System account has been successfully changed.</p>

				<div style=""background-color: #f0fdf4; border-left: 4px solid #22c55e; padding: 20px; margin: 20px 0; border-radius: 4px;"">
					<h3 style=""margin: 0 0 15px 0; color: #166534; font-size: 18px;"">✅ Change Details</h3>
					<table style=""width: 100%; color: #166534; font-size: 15px;"">
						<tr>
							<td style=""padding: 8px 0;""><strong>Date/Time:</strong></td>
							<td style=""padding: 8px 0;"">{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</td>
						</tr>
						<tr>
							<td style=""padding: 8px 0;""><strong>Account:</strong></td>
							<td style=""padding: 8px 0;"">{recipientEmail}</td>
						</tr>
					</table>
				</div>

				<div style=""background-color: #fef2f2; border-left: 4px solid #ef4444; padding: 15px; margin: 20px 0; border-radius: 4px;"">
					<p style=""margin: 0; color: #991b1b;""><strong>⚠️ Security Alert:</strong> If you did not make this change, please contact the system administrator immediately as your account may have been compromised.</p>
				</div>

				<p style=""margin-top: 30px;"">Best regards,<br><strong>PHR System Team</strong></p>
			";
			var htmlBody = CreateHtmlTemplate("Password Changed Successfully", content);
			await SendEmailAsync(recipientEmail, subject, htmlBody);
		}
		public async Task SendAccountCreatedEmailAsync(string recipientEmail, string recipientName, string temporaryPassword)
		{
			var subject = "Your PHR System Account Has Been Created";
			var content = $@"
				<p>Dear <strong>{recipientName}</strong>,</p>
				<p>An administrator has created an account for you in the PHR System.</p>

				<div style=""background-color: #eff6ff; border-left: 4px solid #3b82f6; padding: 20px; margin: 20px 0; border-radius: 4px;"">
					<h3 style=""margin: 0 0 15px 0; color: #1e40af; font-size: 18px;"">🔑 Login Credentials</h3>
					<table style=""width: 100%; color: #1e3a8a; font-size: 15px;"">
						<tr>
							<td style=""padding: 8px 0; width: 180px;""><strong>Email:</strong></td>
							<td style=""padding: 8px 0;"">{recipientEmail}</td>
						</tr>
						<tr>
							<td style=""padding: 8px 0; vertical-align: top;""><strong>Temporary Password:</strong></td>
							<td style=""padding: 8px 0;"">
								<div style=""background-color: #dbeafe; padding: 12px; border-radius: 4px; font-family: 'Courier New', monospace; font-size: 16px; font-weight: bold; color: #1e40af;"">{temporaryPassword}</div>
							</td>
						</tr>
					</table>
				</div>

				<div style=""background-color: #fef3c7; border-left: 4px solid #f59e0b; padding: 15px; margin: 20px 0; border-radius: 4px;"">
					<p style=""margin: 0; color: #92400e;""><strong>⚠️ IMPORTANT:</strong> You will be required to change your password upon first login for security purposes.</p>
				</div>

				<div style=""background-color: #f7fafc; padding: 20px; margin: 20px 0; border-radius: 4px; border: 1px solid #e2e8f0;"">
					<h3 style=""margin: 0 0 15px 0; color: #2d3748; font-size: 16px;"">🚀 To get started:</h3>
					<ol style=""margin: 0; padding-left: 20px; color: #4a5568; line-height: 2;"">
						<li>Navigate to the PHR System login page</li>
						<li>Use the credentials provided above</li>
						<li>You will be prompted to set a new password</li>
						<li>Choose a strong password that meets the system requirements</li>
					</ol>
				</div>

				<p>If you have any questions or did not expect this account creation, please contact your system administrator.</p>
				<p style=""margin-top: 30px;"">Best regards,<br><strong>PHR System Team</strong></p>
			";
			var htmlBody = CreateHtmlTemplate("Account Created Successfully 🎉", content);
			await SendEmailAsync(recipientEmail, subject, htmlBody);
		}

		private string CreateHtmlTemplate(string title, string content)
		{
			return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7fa;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f4f7fa; padding: 20px 0;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; border-collapse: collapse; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: 600; letter-spacing: 0.5px;"">
                                🏥 PHR System
                            </h1>
                            <p style=""margin: 10px 0 0 0; color: #e0e7ff; font-size: 14px;"">Patient Health Records Management</p>
                        </td>
                    </tr>

                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""margin: 0 0 20px 0; color: #1a202c; font-size: 24px; font-weight: 600;"">{title}</h2>
                            <div style=""color: #4a5568; font-size: 16px; line-height: 1.6;"">
                                {content}
                            </div>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f7fafc; padding: 30px; text-align: center; border-top: 1px solid #e2e8f0;"">
                            <p style=""margin: 0 0 10px 0; color: #718096; font-size: 14px;"">
                                This email was sent by PHR System
                            </p>
                            <p style=""margin: 0; color: #a0aec0; font-size: 12px;"">
                                If you have any questions, please contact your system administrator.
                            </p>
                            <p style=""margin: 15px 0 0 0; color: #a0aec0; font-size: 12px;"">
                                © 2025 PHR System. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
		}

		private async Task SendEmailAsync(string recipientEmail, string subject, string htmlBody)
		{
			try
			{
				_logger.LogInformation("Sending email to {Email} with subject: {Subject}", recipientEmail, subject);
				if (string.IsNullOrEmpty(_smtpSettings.Host) || string.IsNullOrEmpty(_smtpSettings.Username))
				{
					_logger.LogWarning("SMTP not configured. Email will be logged only.");
					_logger.LogInformation("Email content - To: {Email}, Subject: {Subject}", recipientEmail, subject);
					return;
				}
				var message = new MimeMessage();
				message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
				message.To.Add(new MailboxAddress("", recipientEmail));
				message.Subject = subject;
				var bodyBuilder = new BodyBuilder
				{
					HtmlBody = htmlBody
				};
				message.Body = bodyBuilder.ToMessageBody();
				using var client = new SmtpClient();
				var secureSocketOptions = _smtpSettings.EnableSsl
					? SecureSocketOptions.StartTls
					: SecureSocketOptions.None;
				await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, secureSocketOptions);
				await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);
				_logger.LogInformation("Email sent successfully to {Email}", recipientEmail);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to send email to {Email}. Error: {Error}", recipientEmail, ex.Message);
				_logger.LogInformation("Failed email - To: {Email}, Subject: {Subject}", recipientEmail, subject);
			}
		}
	}
}
