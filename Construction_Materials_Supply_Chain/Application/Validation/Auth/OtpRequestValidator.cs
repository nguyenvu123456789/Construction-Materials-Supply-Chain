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
    public class OtpRequestValidator : AbstractValidator<OtpRequestDto>
    {
        public OtpRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(AuthMessages.EMAIL_REQUIRED)
                .EmailAddress().WithMessage(AuthMessages.EMAIL_INVALID);

            RuleFor(x => x.Purpose).NotEmpty().WithMessage(AuthMessages.PURPOSE_REQUIRED);
        }
    }
}
