using MediatR;
namespace PHR.Application.Auth.Commands
{
	public record LoginCommand(string Email, string Password) : IRequest<string>;
}