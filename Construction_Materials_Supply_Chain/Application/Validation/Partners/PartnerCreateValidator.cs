using Application.DTOs.Partners;
using FluentValidation;

namespace Application.Validation.Partners
{
    public class PartnerCreateValidator : AbstractValidator<PartnerCreateDto>
    {
        public PartnerCreateValidator()
        {
            RuleFor(x => x.PartnerName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.PartnerTypeId).GreaterThan(0);
            RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
            RuleFor(x => x.ContactPhone).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.ContactPhone));
        }
    }
}
