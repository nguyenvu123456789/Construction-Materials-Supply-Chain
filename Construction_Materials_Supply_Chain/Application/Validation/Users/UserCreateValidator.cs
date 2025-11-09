using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Users
{
    public class UserCreateValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
            RuleFor(x => x.ZaloUserId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.ZaloUserId));
        }
    }
}
