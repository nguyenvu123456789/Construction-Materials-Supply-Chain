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
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordWithOtpDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.OtpCode).NotEmpty().WithMessage(AuthMessages.OTP_REQUIRED);
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage(AuthMessages.PASSWORD_REQUIRED);
        }
    }
}
