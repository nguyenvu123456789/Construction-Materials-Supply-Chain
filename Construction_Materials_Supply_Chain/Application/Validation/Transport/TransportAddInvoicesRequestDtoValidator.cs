using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
