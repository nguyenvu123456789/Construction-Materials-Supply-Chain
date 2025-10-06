using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportsController : ControllerBase
    {
        private readonly IImportService _service;
        private readonly IMapper _mapper;

        public ImportsController(IImportService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult Import([FromBody] ImportRequestDto request)
        {
            try
            {
                var invoice = _service.ImportByCode(request.InvoiceCode, request.WarehouseId, request.CreatedBy);
                return Ok(new { Message = "Nhập kho thành công", InvoiceId = invoice.InvoiceId });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
