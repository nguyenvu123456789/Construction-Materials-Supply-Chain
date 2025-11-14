using System;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/supply-chain-analysis")]
    public class SupplyChainAnalysisController : ControllerBase
    {
        private readonly ISupplyChainAnalysisService _service;

        public SupplyChainAnalysisController(ISupplyChainAnalysisService service)
        {
            _service = service;
        }

        [HttpGet("categories")]
        public IActionResult GetCategorySummary([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var data = _service.GetCategorySummary(from, to);
            return Ok(data);
        }

        [HttpGet("trend")]
        public IActionResult GetSalesTrend([FromQuery] SalesTrendFilterDto filter)
        {
            var data = _service.GetSalesTrend(filter);
            return Ok(data);
        }

        [HttpGet("locations")]
        public IActionResult GetLocationSummary([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var data = _service.GetLocationSummary(from, to);
            return Ok(data);
        }

        [HttpGet("inventory")]
        public IActionResult GetInventorySummary([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int? partnerId)
        {
            var data = _service.GetInventorySummary(from, to, partnerId);
            return Ok(data);
        }

        [HttpGet("recommendations")]
        public IActionResult GetRecommendations([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int? partnerId)
        {
            var data = _service.GetRecommendations(from, to, partnerId);
            return Ok(data);
        }

        [HttpGet("forecast")]
        public IActionResult GetDemandForecast([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] TimeGranularity granularity, [FromQuery] int? materialId, [FromQuery] int? partnerId)
        {
            var data = _service.GetDemandForecast(from, to, granularity, materialId, partnerId);
            return Ok(data);
        }
    }
}
