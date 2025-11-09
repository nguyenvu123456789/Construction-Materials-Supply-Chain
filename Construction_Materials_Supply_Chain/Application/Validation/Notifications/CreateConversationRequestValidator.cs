using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class CreateConversationRequestValidator : AbstractValidator<CreateConversationRequestDto>
    {
        public CreateConversationRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.CreatedByUserId).GreaterThan(0);
            RuleFor(x => x.RecipientUserIds).Must(x => x == null || x.Distinct().Count() == x.Length);
            RuleFor(x => x.RecipientRoleIds).Must(x => x == null || x.Distinct().Count() == x.Length);
        }
    }
}
