using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Common.Pagination;
using Application.Interfaces;
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
    }
}
