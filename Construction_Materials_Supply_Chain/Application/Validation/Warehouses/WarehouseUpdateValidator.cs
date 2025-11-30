using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Warehouses
{
    public class WarehouseUpdateValidator : AbstractValidator<WarehouseUpdateDto>
    {
        public WarehouseUpdateValidator()
        {
            RuleFor(x => x.WarehouseName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Location).MaximumLength(255).When(x => !string.IsNullOrWhiteSpace(x.Location));
            RuleFor(x => x.ManagerId).GreaterThan(0).When(x => x.ManagerId.HasValue);
        }
    }
}
