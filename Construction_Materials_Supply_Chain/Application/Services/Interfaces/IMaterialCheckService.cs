using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Application.DTOs;
using Application.Responses;

namespace Application.Interfaces
{
    public interface IMaterialCheckService
    {
        ApiResponse<MaterialCheckResponseDto> CreateMaterialCheck(MaterialCheckCreateDto dto);
        ApiResponse<MaterialCheckHandleResponseDto> HandleMaterialCheck(MaterialCheckHandleDto dto);
        ApiResponse<PagedResultDto<MaterialCheckResponseDto>> GetAllMaterialChecks(
            int? partnerId = null,
            int? userId = null,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10
        );
        ApiResponse<MaterialCheckResponseWithHandleDto> GetMaterialCheckById(int checkId);
    }
}
