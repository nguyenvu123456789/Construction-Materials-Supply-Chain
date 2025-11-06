using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/alerts/lowstock")]
    public class LowStockAlertsController : ControllerBase
    {
        private readonly ILowStockAlertService _svc;
        public LowStockAlertsController(ILowStockAlertService svc) { _svc = svc; }

        [HttpPost("rules")]
        public IActionResult Create([FromBody] AlertRuleCreateDto dto)
        {
            var id = _svc.Create(dto);
            return Ok(new { id });
        }

        [HttpPut("rules")]
        public IActionResult Update([FromBody] AlertRuleUpdateDto dto)
        {
            _svc.Update(dto);
            return NoContent();
        }

        [HttpPatch("rules/{partnerId:int}/{ruleId:int}/{enable:bool}")]
        public IActionResult Toggle([FromRoute] int partnerId, [FromRoute] int ruleId, [FromRoute] bool enable)
        {
            _svc.Toggle(ruleId, partnerId, enable);
            return NoContent();
        }

        [HttpPost("run")]
        public IActionResult Run([FromBody] RunAlertDto dto)
        {
            _svc.RunOnce(dto.PartnerId);
            return Accepted();
        }
    }
}
