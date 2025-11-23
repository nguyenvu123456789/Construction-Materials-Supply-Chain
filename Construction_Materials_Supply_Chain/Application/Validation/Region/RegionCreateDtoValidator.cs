using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Region
{
    public class RegionCreateDtoValidator : AbstractValidator<RegionCreateDto>
    {
        public RegionCreateDtoValidator()
        {
            RuleFor(x => x.RegionName)
                .NotEmpty().WithMessage("RegionName is required")
                .MaximumLength(200).WithMessage("RegionName is too long");
        }
    }
}
