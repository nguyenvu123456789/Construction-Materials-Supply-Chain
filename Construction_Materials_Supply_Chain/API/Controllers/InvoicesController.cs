using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost("create")]
        public IActionResult CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            if (dto == null) return BadRequest("Invalid request data");

            try
            {
                var invoice = _invoiceService.CreateInvoice(dto);
                return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceId }, invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetInvoice(int id)
        {
            var invoice = _invoiceService.GetByIdWithDetails(id); 
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        [HttpGet]
        public IActionResult GetAllInvoices()
        {
            var invoices = _invoiceService.GetAllWithDetails(); 
            return Ok(invoices);
        }
    }
}