using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Orders
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.CreatedBy).GreaterThan(0);
            RuleFor(x => x.SupplierId).GreaterThan(0);
            RuleFor(x => x.DeliveryAddress).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.DeliveryAddress));
            RuleFor(x => x.Note).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Note));
            RuleFor(x => x.PhoneNumber).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleForEach(x => x.Materials).ChildRules(material =>
            {
                material.RuleFor(m => m.MaterialId).GreaterThan(0);
                material.RuleFor(m => m.Quantity).GreaterThan(0);
            });
        }
    }
}
