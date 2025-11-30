using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Orders
{
    public class HandleOrderValidator : AbstractValidator<HandleOrderRequestDto>
    {
        public HandleOrderValidator()
        {
            RuleFor(x => x.OrderId).GreaterThan(0);
            RuleFor(x => x.HandledBy).GreaterThan(0);
            RuleFor(x => x.ActionType)
                .NotEmpty()
                .Must(action => action == "Approved" || action == "Rejected");
            RuleFor(x => x.Note).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Note));
        }
    }
}
