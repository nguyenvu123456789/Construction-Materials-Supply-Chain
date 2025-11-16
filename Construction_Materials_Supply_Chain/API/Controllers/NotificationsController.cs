using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpPost("conversation")]
        public ActionResult<NotificationResponseDto> CreateConversation([FromBody] CreateConversationRequestDto request)
        {
            var result = notificationService.CreateConversation(request);
            return Ok(result);
        }

        [HttpPost("alert")]
        public ActionResult<NotificationResponseDto> CreateAlert([FromBody] CreateAlertRequestDto request)
        {
            var result = notificationService.CreateAlert(request);
            return Ok(result);
        }

        [HttpPost("recipients/users")]
        public IActionResult AddRecipientsByUsers([FromBody] AckReadCloseRequestDto key, [FromQuery] int[] userIds)
        {
            notificationService.AddRecipientsByUsers(key.NotificationId, key.PartnerId, userIds);
            return NoContent();
        }

        [HttpPost("recipients/roles")]
        public IActionResult AddRecipientsByRoles([FromBody] AckReadCloseRequestDto key, [FromQuery] int[] roleIds)
        {
            notificationService.AddRecipientsByRoles(key.NotificationId, key.PartnerId, roleIds);
            return NoContent();
        }

        [HttpPost("reply")]
        public IActionResult Reply([FromBody] ReplyRequestDto request)
        {
            notificationService.Reply(request);
            return NoContent();
        }

        [HttpPost("read")]
        public IActionResult MarkRead([FromBody] AckReadCloseRequestDto request)
        {
            notificationService.MarkRead(request);
            return NoContent();
        }

        [HttpPost("ack")]
        public IActionResult Acknowledge([FromBody] AckReadCloseRequestDto request)
        {
            notificationService.Acknowledge(request);
            return NoContent();
        }

        [HttpPost("close")]
        public IActionResult Close([FromBody] AckReadCloseRequestDto request)
        {
            notificationService.Close(request);
            return NoContent();
        }

        [HttpGet("{partnerId:int}/{id:int}")]
        public ActionResult<NotificationResponseDto> GetById([FromRoute] int partnerId, [FromRoute] int id)
        {
            var result = notificationService.GetById(id, partnerId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("{partnerId:int}/list")]
        public ActionResult<List<NotificationResponseDto>> GetByPartner([FromRoute] int partnerId)
        {
            var result = notificationService.GetByPartner(partnerId);
            return Ok(result);
        }

        [HttpPost("broadcast/alert")]
        public IActionResult SendCrossPartnerAlert([FromBody] CrossPartnerAlertRequestDto request)
        {
            notificationService.SendCrossPartnerAlert(request);
            return Accepted();
        }

        [HttpGet("{partnerId:int}/user/{userId:int}")]
        public ActionResult<List<NotificationResponseDto>> GetForUser([FromRoute] int partnerId, [FromRoute] int userId)
        {
            var result = notificationService.GetForUser(partnerId, userId);
            return Ok(result);
        }

        [HttpGet("{partnerId:int}/user/{userId:int}/unread-count")]
        public ActionResult<int> CountUnreadForUser([FromRoute] int partnerId, [FromRoute] int userId)
        {
            var result = notificationService.CountUnreadForUser(partnerId, userId);
            return Ok(result);
        }

        [HttpPost("events/trigger")]
        public IActionResult TriggerEvent([FromBody] EventNotifyTriggerDto request)
        {
            notificationService.TriggerEvent(request);
            return Accepted();
        }

        [HttpGet("events/{partnerId:int}/{eventType}")]
        public ActionResult<EventNotifySettingDto> GetEventSetting([FromRoute] int partnerId, [FromRoute] string eventType)
        {
            var setting = notificationService.GetEventSetting(partnerId, eventType);
            if (setting == null) return NotFound();
            return Ok(setting);
        }

        [HttpGet("events/{partnerId:int}")]
        public ActionResult<IEnumerable<EventNotifySettingDto>> GetEventSettings([FromRoute] int partnerId)
        {
            var settings = notificationService.GetEventSettings(partnerId);
            return Ok(settings);
        }

        [HttpPut("events/settings")]
        public IActionResult UpsertEventSetting([FromBody] EventNotifySettingUpsertDto request)
        {
            notificationService.UpsertEventSetting(request);
            return NoContent();
        }

        [HttpPatch("events/settings/{partnerId:int}/{settingId:int}/{isActive:bool}")]
        public IActionResult ToggleEventSetting([FromRoute] int partnerId, [FromRoute] int settingId, [FromRoute] bool isActive)
        {
            notificationService.ToggleEventSetting(settingId, partnerId, isActive);
            return NoContent();
        }

        [HttpDelete("events/settings/{partnerId:int}/{settingId:int}")]
        public IActionResult DeleteEventSetting([FromRoute] int partnerId, [FromRoute] int settingId)
        {
            notificationService.DeleteEventSetting(settingId, partnerId);
            return NoContent();
        }

        [HttpGet("lowstock/rules/{partnerId:int}")]
        public ActionResult<IEnumerable<AlertRuleUpdateDto>> GetLowStockRules([FromRoute] int partnerId)
        {
            var rules = notificationService.GetLowStockRules(partnerId);
            return Ok(rules);
        }

        [HttpPost("lowstock/rules")]
        public ActionResult<object> CreateLowStockRule([FromBody] AlertRuleCreateDto request)
        {
            var id = notificationService.CreateLowStockRule(request);
            return Ok(new { id });
        }

        [HttpPut("lowstock/rules")]
        public IActionResult UpdateLowStockRule([FromBody] AlertRuleUpdateDto request)
        {
            notificationService.UpdateLowStockRule(request);
            return NoContent();
        }

        [HttpPatch("lowstock/rules/{partnerId:int}/{ruleId:int}/{isActive:bool}")]
        public IActionResult ToggleLowStockRule([FromRoute] int partnerId, [FromRoute] int ruleId, [FromRoute] bool isActive)
        {
            notificationService.ToggleLowStockRule(ruleId, partnerId, isActive);
            return NoContent();
        }

        [HttpDelete("lowstock/rules/{partnerId:int}/{ruleId:int}")]
        public IActionResult DeleteLowStockRule([FromRoute] int partnerId, [FromRoute] int ruleId)
        {
            notificationService.DeleteLowStockRule(ruleId, partnerId);
            return NoContent();
        }

        [HttpPost("lowstock/run")]
        public IActionResult RunLowStockAlerts([FromBody] RunAlertDto request)
        {
            notificationService.RunLowStockAlerts(request);
            return Accepted();
        }
    }
}
