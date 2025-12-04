using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Accountant
{
    public class PaymentCreateDtoValidator : AbstractValidator<PaymentCreateDto>
    {
        public PaymentCreateDtoValidator()
        {
            RuleFor(x => x.VendorName).NotEmpty().WithMessage("Tên nhà cung cấp không được để trống.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0.");
            RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Hình thức thanh toán không được để trống.");
            RuleFor(x => x.InvoiceIds).NotEmpty().WithMessage("Hóa đơn liên quan không được để trống.");
        }
    }
}
