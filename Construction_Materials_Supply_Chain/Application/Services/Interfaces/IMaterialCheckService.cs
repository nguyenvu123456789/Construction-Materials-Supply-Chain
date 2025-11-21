using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Common.Pagination;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IMaterialCheckService
    {
        //List<MaterialCheck> GetAll();
        //MaterialCheck? GetById(int id);
        //void Create(MaterialCheck check);
        //void Update(MaterialCheck check);
        //void Delete(int id);
        StockCheckSummaryDto GetSummary(StockCheckQueryDto q);
        PagedResultDto<StockCheckListItemDto> GetChecks(StockCheckQueryDto q);
        PagedResultDto<SkuDiffDto> GetSkuDiffs(StockCheckQueryDto q);
    }
}
