using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Transport
{
    public class TransportAssignRequestDtoValidator : AbstractValidator<TransportAssignRequestDto>
    {
        public TransportAssignRequestDtoValidator()
        {
            RuleFor(x => x.VehicleId).GreaterThan(0);
            RuleFor(x => x.DriverId).GreaterThan(0);
        }
    }
}
