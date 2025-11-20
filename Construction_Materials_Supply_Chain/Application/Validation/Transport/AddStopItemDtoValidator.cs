using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Transport
{
    public class AddStopItemDtoValidator : AbstractValidator<AddStopItemDto>
    {
        private static readonly string[] AllowedTypes = { "Depot", "Pickup", "Dropoff" };

        public AddStopItemDtoValidator()
        {
            RuleFor(x => x.Seq).GreaterThanOrEqualTo(0);
            RuleFor(x => x.StopType)
                .NotEmpty()
                .Must(v => AllowedTypes.Contains(v, StringComparer.OrdinalIgnoreCase));
            RuleFor(x => x.AddressId).GreaterThan(0);
            RuleFor(x => x.ServiceTimeMin).GreaterThanOrEqualTo(0);
        }
    }
}
