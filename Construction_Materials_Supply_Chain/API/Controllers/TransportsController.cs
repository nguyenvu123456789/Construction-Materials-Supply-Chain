using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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
        public IActionResult Assign(int id, [FromBody] TransportAssignRequestDto dto)
        {
            _service.Assign(id, dto);
            return Ok();
        }

        [HttpPost("{id:int}/stops")]
        public IActionResult AddStops(int id, [FromBody] TransportAddStopsRequestDto dto)
        {
            _service.AddStops(id, dto);
            return Ok();
        }

        [HttpPut("{id:int}/stops/{stopId:int}/invoices")]
        public IActionResult SetStopInvoices(int id, int stopId, [FromBody] TransportAddInvoicesRequestDto dto)
        {
            _service.SetStopInvoices(id, stopId, dto.InvoiceIds ?? new List<int>());
            return Ok();
        }

        [HttpPost("{id:int}/stops/proof-base64")]
        public IActionResult UploadStopProofBase64(int id, [FromBody] TransportStopProofBase64Dto dto)
        {
            _service.UploadStopProofBase64(id, dto);
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

        [HttpGet("by-invoice/{invoiceId:int}")]
        public ActionResult<TransportInvoiceTrackingDto> TrackByInvoice(int invoiceId)
        {
            var dto = _service.TrackInvoice(invoiceId);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("history")]
        public ActionResult<List<CustomerOrderStatusDto>> GetHistory([FromQuery] int partnerId)
        {
            var data = _service.GetHistory(partnerId);
            return Ok(data);
        }
    }
}
