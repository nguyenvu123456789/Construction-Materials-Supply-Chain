using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CreateReceipt([FromBody] ReceiptDTO dto)
        {
            _receiptService.CreateReceipt(dto);
            return Ok(new { Message = "Tạo phiếu thu thành công!" });
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDTO dto)
        {
            _paymentService.CreatePayment(dto);
            return Ok(new { Message = "Tạo phiếu chi thành công!" });
        }

        [HttpGet("get-receipt/{id}")]
        public IActionResult GetReceiptById(int id)
        {
            var receipt = _receiptService.GetReceiptById(id);
            if (receipt == null) return NotFound();
            return Ok(receipt);
        }

        [HttpGet("get-payment/{id}")]
        public IActionResult GetPaymentById(int id)
        {
            var payment = _paymentService.GetPaymentById(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        [HttpGet("get-all-receipts")]
        public IActionResult GetAllReceipts()
        {
            return Ok(_receiptService.GetAllReceipts());
        }

        [HttpGet("get-all-payments")]
        public IActionResult GetAllPayments()
        {
            return Ok(_paymentService.GetAllPayments());
        }

        [HttpGet("get-receipts-by-partner/{partnerId}")]
        public IActionResult GetReceiptsByPartnerId(int partnerId)
        {
            var receipts = _receiptService.GetReceiptsByPartnerId(partnerId);
            if (receipts == null || receipts.Count == 0) return NotFound("No receipts found.");
            return Ok(receipts);
        }

        [HttpGet("get-payments-by-partner/{partnerId}")]
        public IActionResult GetPaymentsByPartnerId(int partnerId)
        {
            var payments = _paymentService.GetPaymentsByPartnerId(partnerId);
            if (payments == null || payments.Count == 0) return NotFound("No payments found.");
            return Ok(payments);
        }

        [HttpGet("export-receipts")]
        public async Task<IActionResult> ExportReceipts()
        {
            var fileContent = await _receiptService.ExportReceiptsToExcelAsync();
            string fileName = $"Receipts_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("import-receipts")]
        public async Task<IActionResult> ImportReceipts(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File không hợp lệ.");
            try
            {
                await _receiptService.ImportReceiptsFromExcelAsync(file);
                return Ok(new { Message = "Import phiếu thu thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi import: {ex.Message}");
            }
        }

        [HttpGet("export-payments")]
        public async Task<IActionResult> ExportPayments()
        {
            var fileContent = await _paymentService.ExportPaymentsToExcelAsync();
            string fileName = $"Payments_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("import-payments")]
        public async Task<IActionResult> ImportPayments(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File không hợp lệ.");
            try
            {
                await _paymentService.ImportPaymentsFromExcelAsync(file);
                return Ok(new { Message = "Import phiếu chi thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi import: {ex.Message}");
            }
        }
    }
}