using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LedgerController : ControllerBase
    {
        private readonly IAccountingService _accountingService;

        public LedgerController(IAccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        [HttpGet("general-ledger")]
        public IActionResult GetLedger(DateTime from, DateTime to, int? partnerId)
        {
            return Ok(_accountingService.GetLedger(from, to, partnerId));
        }

        [HttpGet("ap-aging")]
        public IActionResult GetAging(int? partnerId)
        {
            return Ok(_accountingService.GetAPAging(partnerId));
        }

        [HttpGet("cashbook")]
        public IActionResult GetCashBook(DateTime from, DateTime to, int? partnerId)
        {
            return Ok(_accountingService.GetCashBook(from, to, partnerId));
        }
    }
}
