using Application.Constants.Messages;
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
            if (export == null)
                return NotFound(ExportMessages.EXPORT_NOT_FOUND);

            var result = _mapper.Map<ExportResponseDto>(export);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAllExports([FromQuery] int? partnerId = null, [FromQuery] int? managerId = null)
        {
            try
            {
                List<ExportResponseDto> exports;

                if (partnerId.HasValue || managerId.HasValue)
                {
                    exports = _exportService.GetExportsByPartnerOrManager(partnerId, managerId);
                }
                else
                {
                    exports = _exportService.GetAllExports();
                }

                return Ok(exports); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //  Tạo Pending Export
        [HttpPost("request")]
        public IActionResult CreatePendingExport([FromBody] ExportRequestDto dto)
        {
            if (dto == null)
                return BadRequest(ExportMessages.INVALID_REQUEST);

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

        //  Tạo Export thực tế (trừ kho)
        [HttpPost]
        public IActionResult ConfirmExport([FromBody] ExportConfirmDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.ExportCode))
                return BadRequest(ExportMessages.INVALID_REQUEST);

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

        //  Cập nhật trạng thái sang Rejected
        [HttpPut("reject/{id}")]
        public IActionResult RejectExport(int id)
        {
            try
            {
                var export = _exportService.RejectExport(id);
                if (export == null)
                    return NotFound(ExportMessages.EXPORT_NOT_FOUND);

                var result = _mapper.Map<ExportResponseDto>(export);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //  Tạo Export từ Invoice
        [HttpPost("from-invoice")]
        public IActionResult CreateExportFromInvoice([FromBody] ExportFromInvoiceDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.InvoiceCode))
                return BadRequest(ExportMessages.INVALID_REQUEST);

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
