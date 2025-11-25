using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Transport
{
    public class TransportAddInvoicesRequestDtoValidator : AbstractValidator<TransportAddInvoicesRequestDto>
    {
        public TransportAddInvoicesRequestDtoValidator()
        {
            RuleFor(x => x.InvoiceIds).NotNull();
        }
    }
}
