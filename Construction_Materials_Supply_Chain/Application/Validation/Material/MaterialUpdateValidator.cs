using Application.DTOs.Material;
using FluentValidation;

namespace Application.Validation.Materials
{
    public class MaterialUpdateValidator : AbstractValidator<UpdateMaterialRequest>
    {
        public MaterialUpdateValidator()
        {
            RuleFor(x => x.MaterialCode).NotEmpty().MaximumLength(100);
            RuleFor(x => x.MaterialName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.Unit).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Status).NotEmpty().MaximumLength(50);
        }
    }
}
