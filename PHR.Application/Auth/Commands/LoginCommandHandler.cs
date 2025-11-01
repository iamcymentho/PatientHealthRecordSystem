using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PHR.Application.Abstractions.Repositories;
using PHR.Application.Auth.Abstractions;
namespace PHR.Application.Auth.Commands
{
	public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
	{
		private readonly IUserRepository _users;
		private readonly IPasswordVerifier _verifier;
		private readonly ITokenIssuer _tokens;
		public LoginCommandHandler(IUserRepository users, IPasswordVerifier verifier, ITokenIssuer tokens)
		{
			_users = users;
			_verifier = verifier;
			_tokens = tokens;
		}
		public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var user = await _users.GetByEmailAsync(request.Email);
				if (user == null || !_verifier.Verify(request.Password, user.PasswordHash))
				{
					throw new ApplicationException("Invalid credentials");
				}
				return _tokens.Issue(user.Id);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Failed to login.", ex);
			}
		}
	}
}