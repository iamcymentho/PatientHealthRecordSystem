using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PHR.Application.Abstractions.Repositories;
using PHR.Application.Auth.Abstractions;
using PHR.Domain.Entities;
namespace PHR.Application.Auth.Commands
{
	public class AdminCreateUserCommandHandler : IRequestHandler<AdminCreateUserCommand, Guid>
	{
		private readonly IUserRepository _users;
		private readonly IRoleRepository _roles;
		private readonly IPasswordHasherAbstraction _hasher;
		public AdminCreateUserCommandHandler(IUserRepository users, IRoleRepository roles, IPasswordHasherAbstraction hasher)
		{
			_users = users;
			_roles = roles;
			_hasher = hasher;
		}
		public async Task<Guid> Handle(AdminCreateUserCommand request, CancellationToken cancellationToken)
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
					Email = request.Email,
					FullName = request.FullName,
					Gender = request.Gender,
					PhoneNumber = request.PhoneNumber,
					PasswordHash = _hasher.Hash(request.DefaultPassword),
					IsActive = request.IsActive,
					RequirePasswordChange = true
				};
				await _users.AddAsync(user);
				var roleIds = await _roles.GetRoleIdsByNamesAsync(request.Roles);
				if (roleIds.Any())
				{
					await _roles.AssignRolesAsync(user.Id, roleIds);
				}
				return user.Id;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Failed to create user.", ex);
			}
		}
	}
}