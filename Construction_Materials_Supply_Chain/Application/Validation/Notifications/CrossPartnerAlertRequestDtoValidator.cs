using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class CrossPartnerAlertRequestDtoValidator : AbstractValidator<CrossPartnerAlertRequestDto>
    {
        public CrossPartnerAlertRequestDtoValidator()
        {
            RuleFor(x => x.SenderPartnerId).GreaterThan(0);
            RuleFor(x => x.SenderUserId).GreaterThan(0);
            RuleFor(x => x.TargetPartnerIds).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}
