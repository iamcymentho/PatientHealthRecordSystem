using MediatR;
namespace PHR.Application.Auth.Commands
{
	public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Unit>;
}