using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/transports")]
    public class TransportsController : ControllerBase
    {
        private readonly ITransportService _service;
        public TransportsController(ITransportService svc) { _service = svc; }

        [HttpPost]
        public ActionResult<TransportResponseDto> Create([FromBody] TransportCreateRequestDto dto)
            => Ok(_service.Create(dto));

        [HttpGet("{id:int}")]
        public ActionResult<TransportResponseDto> Get(int id)
        {
            var dto = _service.Get(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet]
        public ActionResult<List<TransportResponseDto>> List(
            [FromQuery] DateOnly? date,
            [FromQuery] string? status,
            [FromQuery] int? vehicleId,
            [FromQuery(Name = "partnerId")] int? providerPartnerId
        )
        {
            var data = _service.Query(date, status, vehicleId, providerPartnerId);
            return Ok(data);
        }


        [HttpPost("{id:int}/assign")]
        public IActionResult AssignMulti(int id, [FromBody] TransportAssignMultiRequestDto dto)
        {
            _service.AssignMulti(id, dto);
            return Ok();
        }

        [HttpPost("{id:int}/stops")]
        public IActionResult AddStops(int id, [FromBody] TransportAddStopsRequestDto dto)
        {
            _service.AddStops(id, dto);
            return Ok();
        }

        [HttpPost("{id:int}/orders")]
        public IActionResult AddOrders(int id, [FromBody] TransportAddOrdersRequestDto dto)
        {
            _service.AddOrders(id, dto);
            return Ok();
        }

        [HttpPost("{id:int}/start")]
        public IActionResult Start(int id, [FromQuery] DateTimeOffset? at)
        {
            _service.Start(id, at ?? DateTimeOffset.UtcNow);
            return Ok();
        }

        [HttpPost("{id:int}/stop/arrive")]
        public IActionResult Arrive(int id, [FromBody] TransportStopArriveRequestDto dto)
        {
            _service.ArriveStop(id, dto);
            return Ok();
        }

        [HttpPost("{id:int}/stop/done")]
        public IActionResult Done(int id, [FromBody] TransportStopDoneRequestDto dto)
        {
            _service.CompleteStop(id, dto);
            return Ok();
        }

        [HttpDelete("{id:int}/stops/{stopId:int}")]
        public IActionResult DeleteStop(int id, int stopId)
        {
            _service.DeleteStop(id, stopId);
            return Ok();
        }

        [HttpDelete("{id:int}/stops")]
        public IActionResult ClearStops(int id, [FromQuery] bool keepDepot = true)
        {
            _service.ClearStops(id, keepDepot);
            return Ok();
        }

        [HttpPost("{id:int}/complete")]
        public IActionResult Complete(int id, [FromQuery] DateTimeOffset? at)
        {
            _service.Complete(id, at ?? DateTimeOffset.UtcNow);
            return Ok();
        }

        [HttpPost("{id:int}/cancel")]
        public IActionResult Cancel(int id, [FromQuery] string reason)
        {
            _service.Cancel(id, reason);
            return Ok();
        }
    }
}
