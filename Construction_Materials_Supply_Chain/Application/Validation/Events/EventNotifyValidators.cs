using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Events
{
    public class EventNotifySettingUpsertValidator : AbstractValidator<EventNotifySettingUpsertDto>
    {
        public EventNotifySettingUpsertValidator()
        {
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.EventType).NotEmpty().MaximumLength(100);
        }
    }

    public class EventNotifyTriggerValidator : AbstractValidator<EventNotifyTriggerDto>
    {
        public EventNotifyTriggerValidator()
        {
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.CreatedByUserId).GreaterThan(0);
            RuleFor(x => x.EventType).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
        }
    }
}
