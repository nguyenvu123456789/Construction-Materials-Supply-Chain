using Application.Common.Pagination;
using FluentValidation;

namespace Application.Validation.Partners
{
    public class PartnerPagedQueryValidator : AbstractValidator<PartnerPagedQueryDto>
    {
        public PartnerPagedQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        }
    }
}