using Application.Constants.Messages;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportsController : ControllerBase
    {
        private readonly IImportService _importService;
        private readonly IMapper _mapper;

        public ImportsController(IImportService importService, IMapper mapper)
        {
            _importService = importService;
            _mapper = mapper;
        }


        [HttpPost]
        public IActionResult CreateImportFromInvoice([FromBody] ImportRequestDto dto)
        {
            if (dto == null)
                return BadRequest(ImportMessages.MSG_INVALID_REQUEST_DATA);
            try
            {
                Import import;
                if (!string.IsNullOrEmpty(dto.InvoiceCode))
                {
                    if (dto.WarehouseId <= 0)
                        return BadRequest(ImportMessages.MSG_WAREHOUSE_REQUIRED_FOR_INVOICE);

                    import = _importService.CreateImportFromInvoice(
                        importCode: "IMP-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        invoiceCode: dto.InvoiceCode,
                        warehouseId: dto.WarehouseId,
                        createdBy: dto.CreatedBy,
                        notes: dto.Notes
                    );
                }
                else if (!string.IsNullOrEmpty(dto.ImportCode))
                {
                    import = _importService.CreateImportFromImport(
                        importCode: dto.ImportCode,
                        notes: dto.Notes
                    );
                }
                else
                {
                    return BadRequest(ImportMessages.MSG_MISSING_INVOICE_OR_IMPORT);
                }

                var result = _mapper.Map<ImportResponseDto>(import);
                return CreatedAtAction(nameof(GetImport), new { id = import.ImportId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{id:int}")]
        public IActionResult GetImport(int id)
        {
            var import = _importService.GetByIdWithDetails(id);
            if (import == null) return NotFound();
            var result = _mapper.Map<ImportResponseDto>(import);
            return Ok(result);
        }


        [HttpPost("direct-import")]
        public IActionResult CreateDirectionImport([FromBody] CreatePendingImportDto dto)
        {
            if (dto == null)
                return BadRequest(ImportMessages.MSG_INVALID_REQUEST_DATA);
            try
            {
                var import = _importService.CreateDirectionImport(dto.WarehouseId, dto.CreatedBy, dto.Notes, dto.Materials);
                var result = _mapper.Map<PendingImportResponseDto>(import);
                return CreatedAtAction(nameof(GetImport), new { id = import.ImportId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("reject/{id}")]
        public IActionResult RejectImport(int id)
        {
            try
            {
                var import = _importService.RejectImport(id);
                if (import == null)
                    return NotFound(ImportMessages.MSG_IMPORT_NOT_FOUND);

                var result = _mapper.Map<ImportResponseDto>(import);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetImports([FromQuery] int? partnerId = null, [FromQuery] int? managerId = null)
        {
            try
            {
                var imports = _importService.GetImports(partnerId, managerId);
                var result = _mapper.Map<IEnumerable<ImportResponseDto>>(imports);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
