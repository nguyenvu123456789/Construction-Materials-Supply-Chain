using Application.Interfaces;
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

        [HttpGet("partner/{partnerId}")]
        public IActionResult GetInventoryByPartner(int partnerId)
        {
            var result = _inventoryService.GetInventoryByPartner(partnerId);
            return Ok(result);
        }
    }
}
