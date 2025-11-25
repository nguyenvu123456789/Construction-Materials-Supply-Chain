using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Region
{
    public class RegionUpdateDtoValidator : AbstractValidator<RegionUpdateDto>
    {
        public RegionUpdateDtoValidator()
        {
            RuleFor(x => x.RegionName)
                .NotEmpty().WithMessage("RegionName is required")
                .MaximumLength(200).WithMessage("RegionName is too long");
        }
    }
}
