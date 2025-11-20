using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
