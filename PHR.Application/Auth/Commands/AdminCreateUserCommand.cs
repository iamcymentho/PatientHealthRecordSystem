using System;
using System.Collections.Generic;
using MediatR;
using PHR.Domain.Entities.Enums;
namespace PHR.Application.Auth.Commands
{
	public record AdminCreateUserCommand(string Email, string DefaultPassword, string FullName, Gender Gender, string? PhoneNumber, bool IsActive, IReadOnlyList<string> Roles) : IRequest<Guid>;
}