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
            RuleFor(x => x.MaterialIds)
                .NotNull()
                .Must(list => list.Length > 0)
                .WithMessage("MaterialIds must contain at least one material.");
            RuleForEach(x => x.MaterialIds)
                .GreaterThan(0)
                .WithMessage("MaterialId must be greater than 0.");
            RuleFor(x => x.MinQuantity).GreaterThan(0);
            RuleFor(x => x.CriticalMinQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.RecipientMode).InclusiveBetween(0, 2);
        }
    }
}
