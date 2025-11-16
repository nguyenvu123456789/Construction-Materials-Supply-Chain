using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class AckReadCloseRequestDtoValidator : AbstractValidator<AckReadCloseRequestDto>
    {
        public AckReadCloseRequestDtoValidator()
        {
            RuleFor(x => x.NotificationId).GreaterThan(0);
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
