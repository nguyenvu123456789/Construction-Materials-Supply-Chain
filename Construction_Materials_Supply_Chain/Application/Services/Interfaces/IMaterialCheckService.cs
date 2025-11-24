using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Application.DTOs;
using Application.DTOs.Common.Pagination;
using Application.Responses;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IMaterialCheckService
    {
        ApiResponse<MaterialCheckResponseDto> CreateMaterialCheck(MaterialCheckCreateDto dto);
        ApiResponse<MaterialCheckHandleResponseDto> HandleMaterialCheck(MaterialCheckHandleDto dto);
        ApiResponse<List<MaterialCheckResponseDto>> GetAllMaterialChecks(int? partnerId = null);
        ApiResponse<MaterialCheckResponseWithHandleDto> GetMaterialCheckById(int checkId);
        StockCheckSummaryDto GetSummary(StockCheckQueryDto q);
        PagedResultDto<StockCheckListItemDto> GetChecks(StockCheckQueryDto q);
        PagedResultDto<SkuDiffDto> GetSkuDiffs(StockCheckQueryDto q);
    }
}
