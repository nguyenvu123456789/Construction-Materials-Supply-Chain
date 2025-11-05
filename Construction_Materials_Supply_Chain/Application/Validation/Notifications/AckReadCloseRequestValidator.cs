using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Notifications
{
    public class AckReadCloseRequestValidator : AbstractValidator<AckReadCloseRequestDto>
    {
        public AckReadCloseRequestValidator()
        {
            RuleFor(x => x.NotificationId).GreaterThan(0);
            RuleFor(x => x.PartnerId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
