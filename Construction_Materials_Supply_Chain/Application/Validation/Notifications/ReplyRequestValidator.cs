using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class ReplyRequestValidator : AbstractValidator<ReplyRequestDto>
    {
        public ReplyRequestValidator()
        {
            RuleFor(x => x.NotificationId).GreaterThan(0);
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.Message).NotEmpty().MaximumLength(4000);
        }
    }
}
