using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountingController : ControllerBase
    {
        private readonly IAccountingPostingService _posting;
        private readonly IAccountingQueryService _query;

        public AccountingController(IAccountingPostingService posting, IAccountingQueryService query)
        {
            _posting = posting;
            _query = query;
        }

        [HttpPost("post/sales-invoice/{id:int}")]
        public IActionResult PostSalesInvoice(int id) => Ok(_posting.PostSalesInvoice(id));

        [HttpPost("post/sales-invoice/by-code/{code}")]
        public IActionResult PostSalesInvoiceByCode(string code) => Ok(_posting.PostSalesInvoiceByCode(code));

        [HttpPost("post/purchase-invoice/{id:int}")]
        public IActionResult PostPurchaseInvoice(int id) => Ok(_posting.PostPurchaseInvoice(id));

        [HttpPost("post/receipt/{id:int}")]
        public IActionResult PostReceipt(int id) => Ok(_posting.PostReceipt(id));

        [HttpPost("post/payment/{id:int}")]
        public IActionResult PostPayment(int id) => Ok(_posting.PostPayment(id));

        [HttpDelete("unpost/{sourceType}/{sourceId:int}")]
        public IActionResult Unpost(string sourceType, int sourceId) => Ok(_posting.Unpost(sourceType, sourceId));

        [HttpGet("gl")]
        public IActionResult GetGeneralLedger([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string accountCode, [FromQuery] int partnerId)
            => Ok(_query.GetGeneralLedger(from, to, accountCode, partnerId));

        [HttpGet("ar-aging")]
        public IActionResult GetARAging([FromQuery] DateTime asOf, [FromQuery] int? partnerId)
            => Ok(_query.GetARAging(asOf, partnerId));

        [HttpGet("ap-aging")]
        public IActionResult GetAPAging([FromQuery] DateTime asOf, [FromQuery] int? partnerId)
            => Ok(_query.GetAPAging(asOf, partnerId));

        [HttpGet("cashbook")]
        public IActionResult GetCashbook([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string? method, [FromQuery] int? partnerId)
            => Ok(_query.GetCashbook(from, to, method, partnerId));

        [HttpGet("bank-recon/{statementId:int}")]
        public IActionResult GetBankReconciliation(int statementId)
            => Ok(_query.GetBankReconciliation(statementId));
    }
}
