using Application.DTOs;
using FluentValidation;

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
