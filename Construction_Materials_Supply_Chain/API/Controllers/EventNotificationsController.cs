using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/notifications/events")]
    public class EventNotificationsController : ControllerBase
    {
        private readonly IEventNotificationService _svc;
        public EventNotificationsController(IEventNotificationService svc) { _svc = svc; }

        [HttpGet("{partnerId:int}/{eventType}")]
        public ActionResult<EventNotifySettingDto> Get([FromRoute] int partnerId, [FromRoute] string eventType)
        {
            return Ok(_svc.Get(partnerId, eventType));
        }

        [HttpGet("{partnerId:int}")]
        public ActionResult<IEnumerable<EventNotifySettingDto>> GetAll([FromRoute] int partnerId)
        {
            return Ok(_svc.GetAll(partnerId));
        }

        [HttpPut("settings")]
        public IActionResult Upsert([FromBody] EventNotifySettingUpsertDto dto)
        {
            _svc.Upsert(dto);
            return NoContent();
        }

        [HttpPost("trigger")]
        public IActionResult Trigger([FromBody] EventNotifyTriggerDto dto)
        {
            _svc.Trigger(dto);
            return Accepted();
        }
    }
}
