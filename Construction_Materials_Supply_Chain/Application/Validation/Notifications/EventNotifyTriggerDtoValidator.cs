using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class EventNotifyTriggerDtoValidator : AbstractValidator<EventNotifyTriggerDto>
    {
        public EventNotifyTriggerDtoValidator()
        {
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.EventType).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}
