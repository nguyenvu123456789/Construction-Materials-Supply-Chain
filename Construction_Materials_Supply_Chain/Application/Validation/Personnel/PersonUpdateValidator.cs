using Application.Constants.Messages;
using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation.Personnel
{
    public class PersonUpdateValidator : AbstractValidator<PersonUpdateDto>
    {
        public PersonUpdateValidator()
        {
            When(x => !string.IsNullOrEmpty(x.Phone), () =>
            {
                RuleFor(x => x.Phone)
                    .Matches(@"^\d{10,11}$").WithMessage(PersonnelMessages.PHONE_INVALID);
            });
        }
    }
}
