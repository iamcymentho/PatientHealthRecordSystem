using MediatR;
namespace PHR.Application.Auth.Commands
{
	public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<Unit>;
}