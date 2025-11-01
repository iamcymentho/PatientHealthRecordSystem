using FluentValidation;
namespace PHR.Application.Auth.Commands
{
	public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
	{
		public RegisterUserCommandValidator()
		{
			RuleFor(x => x.Email).NotEmpty().EmailAddress();
			RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
			RuleFor(x => x.FullName).NotEmpty().MinimumLength(2);
			RuleFor(x => x.Gender).IsInEnum();
			RuleFor(x => x.PhoneNumber).MaximumLength(30);
		}
	}
}