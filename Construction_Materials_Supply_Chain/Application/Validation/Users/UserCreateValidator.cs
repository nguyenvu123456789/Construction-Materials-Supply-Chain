using Application.Constants.Messages;
using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Users
{
    public class UserCreateValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255);

            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .Matches(@"^\d{10,11}$").WithMessage(UserMessages.PHONE_INVALID)
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.ZaloUserId)
                .MaximumLength(64);
        }
    }
}
