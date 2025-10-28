using Application.Common.Pagination;
using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoriesController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: api/inventories/partner/{partnerId}?SearchTerm=...&PageNumber=1&PageSize=10
        [HttpGet("partner/{partnerId:int}")]
        public IActionResult GetInventoryByPartner(
            int partnerId,
            [FromQuery] PagedQueryDto queryParams)
        {
            try
            {
                var inventories = _inventoryService.GetInventoryByPartnerFiltered(
                    partnerId,
                    queryParams.SearchTerm,
                    queryParams.PageNumber,
                    queryParams.PageSize,
                    out var totalCount
                );

                var result = new PagedResultDto<InventoryInfoDto>
                {
                    Data = inventories,
                    TotalCount = totalCount,
                    PageNumber = queryParams.PageNumber,
                    PageSize = queryParams.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize)
                };

                return Ok(ApiResponse<PagedResultDto<InventoryInfoDto>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
