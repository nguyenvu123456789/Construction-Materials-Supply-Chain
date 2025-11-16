using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class ReplyRequestDtoValidator : AbstractValidator<ReplyRequestDto>
    {
        public ReplyRequestDtoValidator()
        {
            RuleFor(x => x.NotificationId).GreaterThan(0);
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}
