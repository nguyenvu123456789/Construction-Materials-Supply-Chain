using Application.Constants.Messages;
using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Users
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .MaximumLength(255)
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Phone)
                .Matches(@"^\d{10,11}$").WithMessage(UserMessages.PHONE_INVALID)
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.ZaloUserId)
                .MaximumLength(64);
        }
    }
}
