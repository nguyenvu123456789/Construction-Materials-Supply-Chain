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
            RuleFor(p => p.PartnerName).NotEmpty().WithMessage("Tên người nhận tiền không được để trống");
            RuleFor(p => p.Amount).GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0");
            RuleFor(p => p.Reason).NotEmpty().WithMessage("Lý do chi không được để trống");
        }
    }
}
