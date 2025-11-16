using Application.DTOs;
using FluentValidation;

namespace Application.Validation.Notifications
{
    public class RunAlertDtoValidator : AbstractValidator<RunAlertDto>
    {
        public RunAlertDtoValidator()
        {
            RuleFor(x => x.PartnerId).GreaterThan(0);
        }
    }
}
