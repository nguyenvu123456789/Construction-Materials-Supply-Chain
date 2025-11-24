using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Application.DTOs;
using Application.DTOs.Common.Pagination;
using Application.Interfaces;
using Application.Responses;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/material-check")]
    public class MaterialCheckController : ControllerBase
    {
        private readonly IMaterialCheckService _service;
        public MaterialCheckController(IMaterialCheckService service) { _service = service; }

        [HttpGet("summary")]
        public ActionResult<StockCheckSummaryDto> GetSummary([FromQuery] StockCheckQueryDto q)
            => Ok(_service.GetSummary(q));

        [HttpGet("checks")]
        public ActionResult<PagedResultDto<StockCheckListItemDto>> GetChecks([FromQuery] StockCheckQueryDto q)
            => Ok(_service.GetChecks(q));

        [HttpGet("sku-diffs")]
        public ActionResult<PagedResultDto<SkuDiffDto>> GetSkuDiffs([FromQuery] StockCheckQueryDto q)
            => Ok(_service.GetSkuDiffs(q));

        [HttpPost]
        public ActionResult<ApiResponse<MaterialCheck>> Create([FromBody] MaterialCheckCreateDto dto)
        {
            var result = _service.CreateMaterialCheck(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("handle")]
        public ActionResult<ApiResponse<MaterialCheckHandleResponseDto>> Handle([FromBody] MaterialCheckHandleDto dto)
        {
            var result = _service.HandleMaterialCheck(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        public IActionResult GetAll([FromQuery] int? partnerId)
        {
            var result = _service.GetAllMaterialChecks(partnerId);
            return Ok(result);
        }

        [HttpGet("{checkId:int}")]
        public IActionResult GetMaterialCheckById(int checkId)
        {
            var response = _service.GetMaterialCheckById(checkId);
            return Ok(response);
        }
    }
}
