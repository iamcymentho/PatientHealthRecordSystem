using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PHR.Application.Abstractions.Repositories;
using PHR.Application.Abstractions.Services;
using PHR.Application.Auth.Abstractions;
using PHR.Domain.Entities;
namespace PHR.Application.Auth.Commands
{
	public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
	{
		private readonly IUserRepository _users;
		private readonly IPasswordHasherAbstraction _hasher;
		private readonly IEmailService _emailService;
		public RegisterUserCommandHandler(IUserRepository users, IPasswordHasherAbstraction hasher, IEmailService emailService)
		{
			_users = users;
			_hasher = hasher;
			_emailService = emailService;
		}
		public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
		{
			try
			{
				if (await _users.EmailExistsAsync(request.Email))
				{
					throw new ApplicationException("Email already exists");
				}
				var user = new User
				{
					Id = Guid.NewGuid(),
					FullName = request.FullName,
					Email = request.Email,
					Gender = request.Gender,
					PhoneNumber = request.PhoneNumber,
					PasswordHash = _hasher.Hash(request.Password)
				};
				await _users.AddAsync(user);

				// Send welcome email
				await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);

				return user.Id;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Failed to register user.", ex);
			}
		}
	}
}