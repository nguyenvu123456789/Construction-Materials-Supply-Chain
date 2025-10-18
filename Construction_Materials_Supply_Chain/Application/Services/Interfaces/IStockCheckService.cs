using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Common.Pagination;

namespace Application.Interfaces
{
    public interface IStockCheckService
    {
        StockCheckSummaryDto GetSummary(StockCheckQueryDto query);
        PagedResultDto<StockCheckListItemDto> GetChecks(StockCheckQueryDto query);
        PagedResultDto<SkuDiffDto> GetSkuDiffs(StockCheckQueryDto query);
    }
}
