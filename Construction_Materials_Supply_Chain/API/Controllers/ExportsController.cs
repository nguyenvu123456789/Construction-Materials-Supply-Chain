using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportsController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly IMapper _mapper;

        public ExportsController(IExportService exportService, IMapper mapper)
        {
            _exportService = exportService;
            _mapper = mapper;
        }


        [HttpGet("{id}")]
        public IActionResult GetExport(int id)
        {
            var export = _exportService.GetById(id);
            if (export == null) return NotFound();
            var result = _mapper.Map<ExportResponseDto>(export);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAllExports([FromQuery] int? partnerId = null, [FromQuery] int? managerId = null)
        {
            try
            {
                var exports = (partnerId.HasValue || managerId.HasValue)
                    ? _exportService.GetByPartnerOrManager(partnerId, managerId)
                    : _exportService.GetAll();

                var result = _mapper.Map<IEnumerable<ExportResponseDto>>(exports);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // 🔹 Tạo Pending Export
        [HttpPost("request")]
        public IActionResult CreatePendingExport([FromBody] ExportRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request data.");

            try
            {
                var export = _exportService.CreatePendingExport(dto);
                var result = _mapper.Map<ExportResponseDto>(export);
                return CreatedAtAction(nameof(GetExport), new { id = export.ExportId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Tạo Export thực tế (trừ kho)
        [HttpPost]
        public IActionResult ConfirmExport([FromBody] ExportConfirmDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.ExportCode))
                return BadRequest("Invalid request data.");

            try
            {
                var export = _exportService.ConfirmExport(dto.ExportCode, dto.Notes);
                var result = _mapper.Map<ExportResponseDto>(export);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // 🔹 Cập nhật trạng thái sang Rejected
        [HttpPut("reject/{id}")]
        public IActionResult RejectExport(int id)
        {
            try
            {
                var export = _exportService.RejectExport(id);
                if (export == null)
                    return NotFound("Export not found.");

                var result = _mapper.Map<ExportResponseDto>(export);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("from-invoice")]
        public IActionResult CreateExportFromInvoice([FromBody] ExportFromInvoiceDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.InvoiceCode))
                return BadRequest("Invalid request data.");

            try
            {
                var export = _exportService.CreateExportFromInvoice(dto);
                var result = _mapper.Map<ExportResponseDto>(export);
                return CreatedAtAction(nameof(GetExport), new { id = export.ExportId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}