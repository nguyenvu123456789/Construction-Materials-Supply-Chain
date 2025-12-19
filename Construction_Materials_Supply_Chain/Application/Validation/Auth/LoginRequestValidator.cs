using Application.Constants.Messages;
using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Auth
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(AuthMessages.USERNAME_REQUIRED);
            RuleFor(x => x.Password).NotEmpty().WithMessage(AuthMessages.PASSWORD_REQUIRED);
        }
    }
}