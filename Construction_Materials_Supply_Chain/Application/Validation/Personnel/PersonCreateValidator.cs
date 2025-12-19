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
    public class PersonCreateValidator : AbstractValidator<PersonCreateDto>
    {
        public PersonCreateValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .Must(x => new[] { "driver", "porter", "vehicle" }.Contains(x.ToLower().Trim()))
                .WithMessage(PersonnelMessages.TYPE_INVALID);

            // Validate khi là Driver hoặc Porter
            When(x => x.Type != null && (x.Type.Trim().ToLower() == "driver" || x.Type.Trim().ToLower() == "porter"), () =>
            {
                RuleFor(x => x.FullName)
                    .NotEmpty().WithMessage(PersonnelMessages.NAME_REQUIRED)
                    .MaximumLength(100);

                RuleFor(x => x.Phone)
                    .NotEmpty()
                    .Matches(@"^\d{10,11}$").WithMessage(PersonnelMessages.PHONE_INVALID);
            });

            // Validate khi là Vehicle
            When(x => x.Type != null && x.Type.Trim().ToLower() == "vehicle", () =>
            {
                RuleFor(x => x.Code)
                    .NotEmpty().WithMessage(PersonnelMessages.NAME_REQUIRED);

                RuleFor(x => x.PlateNumber)
                    .NotEmpty().WithMessage(PersonnelMessages.PLATE_REQUIRED);
            });
        }
    }
}
