using FluentValidation;
namespace PHR.Application.Auth.Commands
{
	public class AdminCreateUserCommandValidator : AbstractValidator<AdminCreateUserCommand>
	{
		public AdminCreateUserCommandValidator()
		{
			RuleFor(x => x.Email).NotEmpty().EmailAddress();
			RuleFor(x => x.DefaultPassword).NotEmpty().MinimumLength(6);
			RuleFor(x => x.FullName).NotEmpty().MinimumLength(2);
			RuleFor(x => x.Gender).IsInEnum();
			RuleFor(x => x.PhoneNumber).MaximumLength(30);
		}
	}
}