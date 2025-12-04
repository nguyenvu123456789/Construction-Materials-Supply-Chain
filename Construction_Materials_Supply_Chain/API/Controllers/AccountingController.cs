using Application.Services.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountingController : ControllerBase
    {
        private readonly IReceiptService _receiptService;
        private readonly IPaymentService _paymentService;

        public AccountingController(IReceiptService receiptService, IPaymentService paymentService)
        {
            _receiptService = receiptService;
            _paymentService = paymentService;
        }

        [HttpGet("receipts/{partnerId}")]
        public IActionResult GetReceiptsByPartnerId(int partnerId)
        {
            var receipts = _receiptService.GetReceiptsByPartnerId(partnerId);
            return Ok(receipts);
        }

        [HttpGet("receipt/{id}")]
        public IActionResult GetReceiptById(int id)
        {
            var receipt = _receiptService.GetReceiptById(id);
            if (receipt == null)
                return NotFound();
            return Ok(receipt);
        }

        [HttpPost("receipts")]
        public IActionResult AddReceipt([FromBody] ReceiptDTO receiptDTO)
        {
            try
            {
                _receiptService.AddReceipt(receiptDTO);
                return CreatedAtAction(nameof(GetReceiptById), new { id = receiptDTO.ReceiptNumber }, receiptDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("receipts/{id}")]
        public IActionResult UpdateReceipt(int id, [FromBody] ReceiptDTO receiptDTO)
        {
            if (id.ToString() != receiptDTO.ReceiptNumber)
                return BadRequest("ID mismatch");

            try
            {
                _receiptService.UpdateReceipt(receiptDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("receipts/{id}")]
        public IActionResult DeleteReceipt(int id)
        {
            try
            {
                _receiptService.DeleteReceipt(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("payments/{partnerId}")]
        public IActionResult GetPaymentsByPartnerId(int partnerId)
        {
            var payments = _paymentService.GetPaymentsByPartnerId(partnerId);
            return Ok(payments);
        }

        [HttpGet("payment/{id}")]
        public IActionResult GetPaymentById(int id)
        {
            var payment = _paymentService.GetPaymentById(id);
            if (payment == null)
                return NotFound();
            return Ok(payment);
        }

        [HttpPost("payments")]
        public IActionResult AddPayment([FromBody] PaymentDTO paymentDTO)
        {
            try
            {
                _paymentService.AddPayment(paymentDTO);
                return CreatedAtAction(nameof(GetPaymentById), new { id = paymentDTO.PaymentNumber }, paymentDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("payments/{id}")]
        public IActionResult UpdatePayment(int id, [FromBody] PaymentDTO paymentDTO)
        {
            if (id.ToString() != paymentDTO.PaymentNumber)
                return BadRequest("ID mismatch");

            try
            {
                _paymentService.UpdatePayment(paymentDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("payments/{id}")]
        public IActionResult DeletePayment(int id)
        {
            try
            {
                _paymentService.DeletePayment(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
