using System;
using MediatR;
using PHR.Domain.Entities.Enums;
namespace PHR.Application.Auth.Commands
{
	public record RegisterUserCommand(string Email, string Password, string FullName, Gender Gender, string? PhoneNumber) : IRequest<Guid>;
}