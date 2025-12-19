using Application.Constants.Messages;
using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Auth
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.OldPassword).NotEmpty().WithMessage(AuthMessages.PASSWORD_REQUIRED);
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage(AuthMessages.PASSWORD_REQUIRED);
        }
    }
}
