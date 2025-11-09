using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Alerts
{
    public class AlertRuleCreateValidator : AbstractValidator<AlertRuleCreateDto>
    {
        public AlertRuleCreateValidator()
        {
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.MaterialId).GreaterThan(0);
            RuleFor(x => x.MinQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.RecipientMode).InclusiveBetween(1, 3);
        }
    }

    public class AlertRuleUpdateValidator : AbstractValidator<AlertRuleUpdateDto>
    {
        public AlertRuleUpdateValidator()
        {
            Include(new AlertRuleCreateValidator());
            RuleFor(x => x.RuleId).GreaterThan(0);
        }
    }
}
