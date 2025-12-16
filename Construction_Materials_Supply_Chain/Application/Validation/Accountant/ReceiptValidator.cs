using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Accountant
{
    public class ReceiptValidator : AbstractValidator<ReceiptDTO>
    {
        public ReceiptValidator()
        {
            RuleFor(r => r.PartnerName).NotEmpty().WithMessage("Tên người nộp tiền không được để trống");
            RuleFor(r => r.Amount).GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0");
            RuleFor(r => r.Reason).NotEmpty().WithMessage("Lý do thu không được để trống");
        }
    }
}
