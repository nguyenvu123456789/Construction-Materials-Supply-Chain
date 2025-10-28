using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketAnalysisController : ControllerBase
    {
        private readonly IMarketAnalysisService _service;

        public MarketAnalysisController(IMarketAnalysisService service)
        {
            _service = service;
        }

        [HttpGet("revenue/monthly")]
        public IActionResult GetMonthlyRevenue()
        {
            return Ok(_service.GetMonthlyRevenue());
        }

        [HttpGet("top-materials")]
        public IActionResult GetTopMaterials([FromQuery] int top = 5)
        {
            return Ok(_service.GetTopMaterials(top));
        }

        [HttpGet("revenue/supplier")]
        public IActionResult GetRevenueBySupplier()
        {
            return Ok(_service.GetRevenueBySupplier());
        }

        [HttpGet("revenue/staff")]
        public IActionResult GetRevenueByStaff()
        {
            return Ok(_service.GetRevenueByStaff());
        }

        [HttpGet("revenue/region")]
        public IActionResult GetRevenueByRegion()
        {
            return Ok(_service.GetRevenueByRegion());
        }
    }
}
