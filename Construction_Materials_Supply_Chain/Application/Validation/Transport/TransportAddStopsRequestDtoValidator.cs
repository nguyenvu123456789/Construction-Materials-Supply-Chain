using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Transport
{
    public class TransportAddStopsRequestDtoValidator : AbstractValidator<TransportAddStopsRequestDto>
    {
        public TransportAddStopsRequestDtoValidator()
        {
            RuleForEach(x => x.Stops).SetValidator(new AddStopItemDtoValidator());
        }
    }
}
