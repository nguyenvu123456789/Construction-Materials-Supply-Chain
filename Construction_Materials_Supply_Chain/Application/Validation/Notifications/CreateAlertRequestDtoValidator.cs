using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class CreateAlertRequestDtoValidator : AbstractValidator<CreateAlertRequestDto>
    {
        public CreateAlertRequestDtoValidator()
        {
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.CreatedByUserId).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}
