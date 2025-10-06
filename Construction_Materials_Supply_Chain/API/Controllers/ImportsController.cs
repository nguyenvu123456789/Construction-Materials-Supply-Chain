using API.DTOs;
using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportsController : ControllerBase
    {
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public ImportsController(IImportRepository importRepository, IMapper mapper)
        {
            _importRepository = importRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult Import([FromBody] ImportRequestDto request)
        {
            var invoice = _importRepository.GetPendingInvoiceByCode(request.InvoiceCode);
            if (invoice == null)
                return NotFound($"Invoice {request.InvoiceCode} không tồn tại hoặc không ở trạng thái Pending.");

            _importRepository.ImportInvoice(invoice, request.WarehouseId, request.CreatedBy);

            return Ok(new { Message = "Nhập kho thành công", InvoiceId = invoice.InvoiceId });
        }
    }
}
