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


        [HttpPost("from-order")]
        public IActionResult CreateInvoiceFromOrder([FromBody] CreateInvoiceFromOrderDto dto)
        {
            if (dto == null) return BadRequest("Invalid request data");

            try
            {
                var invoices = _invoiceService.CreateInvoiceFromOrder(dto);
                return Ok(new
                {
                    message = $"{invoices.Count} invoice(s) created successfully",
                    invoices
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpPut("reject/{id}")]
        public IActionResult RejectInvoice(int id)
        {
            try
            {
                var invoice = _invoiceService.RejectInvoice(id);
                if (invoice == null)
                    return NotFound("Invoice not found.");

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{invoiceId:int}/partner/{partnerId:int}")]
        public IActionResult GetInvoiceForPartner(int invoiceId, int partnerId)
        {
            var result = _invoiceService.GetInvoiceForPartner(invoiceId, partnerId);
            return Ok(result);
        }

        [HttpGet("partner/{partnerId:int}")]
        public IActionResult GetAllInvoicesForPartner(int partnerId)
        {
            var invoices = _invoiceService.GetAllInvoicesForPartner(partnerId);
            return Ok(invoices);
        }

    }
}