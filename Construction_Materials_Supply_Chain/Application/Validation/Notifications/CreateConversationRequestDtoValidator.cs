using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class CreateConversationRequestDtoValidator : AbstractValidator<CreateConversationRequestDto>
    {
        public CreateConversationRequestDtoValidator()
        {
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.CreatedByUserId).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}
