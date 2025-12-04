using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers
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

        [HttpPost("create-receipt")]
        public async Task<IActionResult> CreateReceipt([FromForm] CreateReceiptRequest request)
        {
            _receiptService.CreateReceipt(request.Receipt, request.AttachmentFile);

            return Ok(new { Message = "Tạo phiếu thu thành công!" });
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromForm] CreatePaymentRequest request)
        {
            _paymentService.CreatePayment(request.Payment, request.AttachmentFile);

            return Ok(new { Message = "Tạo phiếu chi thành công!" });
        }

        [HttpGet("get-receipt/{id}")]
        public IActionResult GetReceiptById(int id)
        {
            var receipt = _receiptService.GetReceiptById(id);
            if (receipt == null)
                return NotFound();
            return Ok(receipt);
        }

        [HttpGet("get-payment/{id}")]
        public IActionResult GetPaymentById(int id)
        {
            var payment = _paymentService.GetPaymentById(id);
            if (payment == null)
                return NotFound();
            return Ok(payment);
        }

        [HttpGet("get-all-receipts")]
        public IActionResult GetAllReceipts()
        {
            var receipts = _receiptService.GetAllReceipts();
            return Ok(receipts);
        }

        [HttpGet("get-all-payments")]
        public IActionResult GetAllPayments()
        {
            var payments = _paymentService.GetAllPayments();
            return Ok(payments);
        }

        [HttpGet("get-receipts-by-partner/{partnerId}")]
        public IActionResult GetReceiptsByPartnerId(int partnerId)
        {
            var receipts = _receiptService.GetReceiptsByPartnerId(partnerId);
            if (receipts == null || receipts.Count == 0)
            {
                return NotFound("No receipts found for the given partner.");
            }

            return Ok(receipts);
        }

        [HttpGet("get-payments-by-partner/{partnerId}")]
        public IActionResult GetPaymentsByPartnerId(int partnerId)
        {
            var payments = _paymentService.GetPaymentsByPartnerId(partnerId);
            if (payments == null || payments.Count == 0)
            {
                return NotFound("No payments found for the given partner.");
            }

            return Ok(payments);
        }
    }
}
