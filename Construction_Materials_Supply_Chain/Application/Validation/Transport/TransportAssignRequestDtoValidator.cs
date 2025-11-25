using Application.DTOs;
using FluentValidation;

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
