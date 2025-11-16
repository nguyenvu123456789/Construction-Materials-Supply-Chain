using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class EventNotifySettingUpsertDtoValidator : AbstractValidator<EventNotifySettingUpsertDto>
    {
        public EventNotifySettingUpsertDtoValidator()
        {
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.EventType).NotEmpty().MaximumLength(200);
            RuleFor(x => x.RoleIds).NotNull();
        }
    }
}
