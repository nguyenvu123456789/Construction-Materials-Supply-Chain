using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Users
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.Email).EmailAddress().MaximumLength(255).When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.ZaloUserId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.ZaloUserId));
        }
    }
}
