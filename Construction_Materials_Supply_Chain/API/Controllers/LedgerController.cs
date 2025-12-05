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
        public IActionResult GetLedger(DateTime from, DateTime to)
        {
            return Ok(_accountingService.GetLedger(from, to));
        }

        [HttpGet("ap-aging")]
        public IActionResult GetAging()
        {
            return Ok(_accountingService.GetAPAging());
        }

        [HttpGet("cashbook")]
        public IActionResult GetCashBook(DateTime from, DateTime to)
        {
            return Ok(_accountingService.GetCashBook(from, to));
        }
    }
}
