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
        public IActionResult CreateImport([FromBody] ImportRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request data");

            try
            {
                Import import;

                // ✅ 1️⃣ Trường hợp nhập theo hóa đơn (InvoiceCode)
                if (!string.IsNullOrEmpty(dto.InvoiceCode))
                {
                    if (dto.WarehouseId <= 0)
                        return BadRequest("WarehouseId is required when importing from invoice.");

                    import = _importService.CreateImportFromInvoice(
                        importCode: "IMP-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        invoiceCode: dto.InvoiceCode,
                        warehouseId: dto.WarehouseId,
                        createdBy: dto.CreatedBy,
                        notes: dto.Notes
                    );
                }
                // ✅ 2️⃣ Trường hợp nhập từ phiếu Pending (ImportCode)
                else if (!string.IsNullOrEmpty(dto.ImportCode))
                {
                    import = _importService.ConfirmPendingImport(
                        importCode: dto.ImportCode,
                        notes: dto.Notes
                    );
                }
                else
                {
                    return BadRequest("You must provide either ImportCode or InvoiceCode.");
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

        [HttpGet]
        public IActionResult GetAll()
        {
            var imports = _importService.GetAll();
            var result = _mapper.Map<IEnumerable<ImportResponseDto>>(imports);
            return Ok(result);
        }

        [HttpPost("request")]
        public IActionResult CreatePendingImport([FromBody] CreatePendingImportDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request data");

            try
            {
                var import = _importService.CreatePendingImport(dto.WarehouseId, dto.CreatedBy, dto.Notes, dto.Materials);
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
                    return NotFound("Import not found.");

                var result = _mapper.Map<ImportResponseDto>(import);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
