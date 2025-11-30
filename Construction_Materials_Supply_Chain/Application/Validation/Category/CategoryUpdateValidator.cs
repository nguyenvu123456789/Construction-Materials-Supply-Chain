using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Categories
{
    public class CategoryUpdateValidator : AbstractValidator<CategoryDto>
    {
        public CategoryUpdateValidator()
        {
            RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
