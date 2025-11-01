using MediatR;
namespace PHR.Application.Auth.Commands
{
	public record ResetPasswordRequestCommand(string Email) : IRequest<Unit>;
}