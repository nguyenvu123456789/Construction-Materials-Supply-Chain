using Application.Constants.Messages;
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
            if (invoice == null) return NotFound(new { message = InvoiceMessages.INVOICE_NOT_FOUND });
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
            if (dto == null) return BadRequest(new { message = InvoiceMessages.INVALID_REQUEST });

            try
            {
                var invoices = _invoiceService.CreateInvoiceFromOrder(dto);
                return Ok(new
                {
                    message = string.Format(InvoiceMessages.INVOICE_CREATED_SUCCESS, invoices.Count),
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
                    return NotFound(new { message = InvoiceMessages.INVOICE_NOT_FOUND });

                return Ok(new { message = InvoiceMessages.INVOICE_REJECTED_SUCCESS, invoice });
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

        [HttpGet("partner")]
        public IActionResult GetAllInvoicesForPartner([FromQuery] int? partnerId, [FromQuery] int? managerId)
        {
            var invoices = _invoiceService.GetAllInvoicesForPartnerOrManager(partnerId, managerId);
            return Ok(invoices);
        }

        [HttpPut("mark-delivered")]
        public IActionResult MarkDelivered([FromBody] UpdateInvoiceStatusDto dto)
        {
            _invoiceService.MarkInvoicesAsDelivered(dto.InvoiceIds);
            return Ok(new
            {
                message = InvoiceMessages.INVOICE_MARK_DELIVERED_SUCCESS
            });
        }

        [HttpGet("by-partner-seller")]
        public IActionResult GetInvoicesSeller([FromQuery] int partnerId)
        {
            var invoices = _invoiceService.GetInvoiceSeller(partnerId);

            if (invoices == null || invoices.Count == 0)
                return NotFound(new { message = InvoiceMessages.INVOICE_NOT_FOUND });

            return Ok(invoices);
        }

        [HttpGet("by-partner-buyer")]
        public IActionResult GetInvoicesBuyer([FromQuery] int partnerId)
        {
            var invoices = _invoiceService.GetInvoiceBuyer(partnerId);

            if (invoices == null || invoices.Count == 0)
                return NotFound(new { message = InvoiceMessages.INVOICE_NOT_FOUND });

            return Ok(invoices);
        }
    }
}
