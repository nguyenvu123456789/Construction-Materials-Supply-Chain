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

        [HttpPost("post/export-cogs/{exportId:int}")]
        public IActionResult PostExportCogs(int exportId) => Ok(_posting.PostExportCogs(exportId));

        [HttpPost("post/receipt/{id:int}")]
        public IActionResult PostReceipt(int id) => Ok(_posting.PostReceipt(id));

        [HttpPost("post/payment/{id:int}")]
        public IActionResult PostPayment(int id) => Ok(_posting.PostPayment(id));

        [HttpDelete("unpost/{sourceType}/{sourceId:int}")]
        public IActionResult Unpost(string sourceType, int sourceId) => Ok(_posting.Unpost(sourceType, sourceId));

        [HttpGet("gl")]
        public IActionResult GetGeneralLedger([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string accountCode)
            => Ok(_query.GetGeneralLedger(from, to, accountCode));

        [HttpGet("ar-aging")]
        public IActionResult GetARAging([FromQuery] DateTime asOf)
            => Ok(_query.GetARAging(asOf));

        [HttpGet("ap-aging")]
        public IActionResult GetAPAging([FromQuery] DateTime asOf)
            => Ok(_query.GetAPAging(asOf));

        [HttpGet("cashbook")]
        public IActionResult GetCashbook([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string? method)
            => Ok(_query.GetCashbook(from, to, method));

        [HttpGet("bank-recon/{statementId:int}")]
        public IActionResult GetBankReconciliation(int statementId)
            => Ok(_query.GetBankReconciliation(statementId));
    }
}
