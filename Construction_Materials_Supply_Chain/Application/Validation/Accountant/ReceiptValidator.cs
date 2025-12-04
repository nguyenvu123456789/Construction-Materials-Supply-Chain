using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Accountant
{
    public class ReceiptValidator : AbstractValidator<ReceiptDTO>
    {
        public ReceiptValidator()
        {
            RuleFor(x => x.ReceiptNumber).NotEmpty().WithMessage("Số phiếu thu không được để trống.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0.");
            RuleFor(x => x.PartnerId).NotEmpty().WithMessage("Khách hàng không được để trống.");
        }
    }
}
