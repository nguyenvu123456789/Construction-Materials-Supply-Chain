using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Categories
{
    public class CategoryCreateValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CategoryCreateValidator()
        {
            RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
