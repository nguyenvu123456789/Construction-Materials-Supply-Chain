using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Accountant
{
    public class PaymentValidator : AbstractValidator<PaymentDTO>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0.");
            RuleFor(x => x.PartnerId).NotEmpty().WithMessage("Nhà cung cấp không được để trống.");
        }
    }
}
