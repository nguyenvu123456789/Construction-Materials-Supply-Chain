using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Accountant
{
    public class ReceiptCreateDtoValidator : AbstractValidator<ReceiptCreateDto>
    {
        public ReceiptCreateDtoValidator()
        {
            RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Tên khách hàng không được để trống.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0.");
            RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Hình thức thanh toán không được để trống.");
            RuleFor(x => x.InvoiceIds).NotEmpty().WithMessage("Hóa đơn liên quan không được để trống.");
        }
    }
}
