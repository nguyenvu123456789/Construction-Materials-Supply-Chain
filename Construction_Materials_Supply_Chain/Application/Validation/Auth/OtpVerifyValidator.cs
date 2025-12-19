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
    public class OtpVerifyValidator : AbstractValidator<OtpVerifyDto>
    {
        public OtpVerifyValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Code).NotEmpty().WithMessage(AuthMessages.OTP_REQUIRED);
            RuleFor(x => x.Purpose).NotEmpty().WithMessage(AuthMessages.PURPOSE_REQUIRED);
        }
    }
}
