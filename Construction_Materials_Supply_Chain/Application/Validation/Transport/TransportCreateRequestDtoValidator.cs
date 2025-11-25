using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Transport
{
    public class TransportCreateRequestDtoValidator : AbstractValidator<TransportCreateRequestDto>
    {
        public TransportCreateRequestDtoValidator()
        {
            RuleFor(x => x.DepotId).GreaterThan(0);
            RuleFor(x => x.ProviderPartnerId).GreaterThan(0);
        }
    }
}
