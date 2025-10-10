using Application.DTOs.Common.Pagination;
using FluentValidation;

namespace Application.Validation.Stock
{
    public class StockCheckQueryValidator : AbstractValidator<StockCheckQueryDto>
    {
        public StockCheckQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
            RuleFor(x => x.To)
                .GreaterThanOrEqualTo(x => x.From)
                .When(x => x.From.HasValue && x.To.HasValue)
                .WithMessage("'To' must be >= 'From'.");
        }
    }
}
