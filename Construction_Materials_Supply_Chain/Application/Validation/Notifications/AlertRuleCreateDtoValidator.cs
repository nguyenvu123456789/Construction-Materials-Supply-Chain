using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class AlertRuleCreateDtoValidator : AbstractValidator<AlertRuleCreateDto>
    {
        public AlertRuleCreateDtoValidator()
        {
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.MaterialId).GreaterThan(0);
            RuleFor(x => x.MinQuantity).GreaterThan(0);
            RuleFor(x => x.CriticalMinQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.RecipientMode).InclusiveBetween(0, 2);
        }
    }
}
