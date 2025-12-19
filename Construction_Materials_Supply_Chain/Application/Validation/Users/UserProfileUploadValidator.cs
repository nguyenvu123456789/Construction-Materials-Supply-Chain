using Application.Constants.Messages;
using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Users
{
    public class UserProfileUploadValidator : AbstractValidator<UserProfileUploadDto>
    {
        public UserProfileUploadValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .Matches(@"^\d{10,11}$").WithMessage(UserMessages.PHONE_INVALID)
                .When(x => !string.IsNullOrEmpty(x.Phone));
        }
    }
}
