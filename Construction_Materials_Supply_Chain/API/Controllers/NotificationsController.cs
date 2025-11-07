using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _svc;

        public NotificationsController(INotificationService svc)
        {
            _svc = svc;
        }

        [HttpPost("conversation")]
        public ActionResult<NotificationResponseDto> CreateConversation([FromBody] CreateConversationRequestDto dto)
        {
            var result = _svc.CreateConversation(dto);
            return Ok(result);
        }

        [HttpPost("alert")]
        public ActionResult<NotificationResponseDto> CreateAlert([FromBody] CreateAlertRequestDto dto)
        {
            var result = _svc.CreateAlert(dto);
            return Ok(result);
        }

        [HttpPost("recipients/users")]
        public IActionResult AddRecipientsByUsers([FromBody] AckReadCloseRequestDto key, [FromQuery] int[] userIds)
        {
            _svc.AddRecipientsByUsers(key.NotificationId, key.PartnerId, userIds);
            return NoContent();
        }

        [HttpPost("recipients/roles")]
        public IActionResult AddRecipientsByRoles([FromBody] AckReadCloseRequestDto key, [FromQuery] int[] roleIds)
        {
            _svc.AddRecipientsByRoles(key.NotificationId, key.PartnerId, roleIds);
            return NoContent();
        }

        [HttpPost("reply")]
        public IActionResult Reply([FromBody] ReplyRequestDto dto)
        {
            _svc.Reply(dto);
            return NoContent();
        }

        [HttpPost("read")]
        public IActionResult MarkRead([FromBody] AckReadCloseRequestDto dto)
        {
            _svc.MarkRead(dto);
            return NoContent();
        }

        [HttpPost("ack")]
        public IActionResult Acknowledge([FromBody] AckReadCloseRequestDto dto)
        {
            _svc.Acknowledge(dto);
            return NoContent();
        }

        [HttpPost("close")]
        public IActionResult Close([FromBody] AckReadCloseRequestDto dto)
        {
            _svc.Close(dto);
            return NoContent();
        }

        [HttpGet("{partnerId:int}/{id:int}")]
        public ActionResult<NotificationResponseDto> GetById([FromRoute] int partnerId, [FromRoute] int id)
        {
            var result = _svc.GetById(id, partnerId);
            return Ok(result);
        }

        [HttpGet("{partnerId:int}/list")]
        public ActionResult<List<NotificationResponseDto>> GetByPartner([FromRoute] int partnerId)
        {
            var result = _svc.GetByPartner(partnerId);
            return Ok(result);
        }

        [HttpPost("broadcast/alert")]
        public IActionResult SendCrossPartnerAlert([FromBody] CrossPartnerAlertRequestDto dto)
        {
            _svc.SendCrossPartnerAlert(dto);
            return Accepted();
        }

        [HttpGet("{partnerId:int}/user/{userId:int}")]
        public ActionResult<List<NotificationResponseDto>> GetForUser([FromRoute] int partnerId, [FromRoute] int userId)
        {
            var result = _svc.GetForUser(partnerId, userId);
            return Ok(result);
        }

        [HttpGet("{partnerId:int}/user/{userId:int}/unread-count")]
        public ActionResult<int> CountUnreadForUser([FromRoute] int partnerId, [FromRoute] int userId)
        {
            var result = _svc.CountUnreadForUser(partnerId, userId);
            return Ok(result);
        }
    }
}
