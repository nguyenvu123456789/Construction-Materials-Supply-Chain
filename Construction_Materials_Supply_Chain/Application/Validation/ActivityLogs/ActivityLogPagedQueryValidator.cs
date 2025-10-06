using Application.Common.Pagination;
using FluentValidation;

namespace Application.Validation.ActivityLogs
{
    public class ActivityLogPagedQueryValidator : AbstractValidator<ActivityLogPagedQueryDto>
    {
        public ActivityLogPagedQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
            When(x => x.FromDate.HasValue && x.ToDate.HasValue, () =>
            {
                RuleFor(x => x.FromDate).LessThanOrEqualTo(x => x.ToDate);
            });
        }
    }
}