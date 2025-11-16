using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class AlertRuleUpdateDtoValidator : AbstractValidator<AlertRuleUpdateDto>
    {
        public AlertRuleUpdateDtoValidator()
        {
            RuleFor(x => x.RuleId).GreaterThan(0);
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.MaterialId).GreaterThan(0);
            RuleFor(x => x.MinQuantity).GreaterThan(0);
            RuleFor(x => x.CriticalMinQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.RecipientMode).InclusiveBetween(0, 2);
        }
    }
}
