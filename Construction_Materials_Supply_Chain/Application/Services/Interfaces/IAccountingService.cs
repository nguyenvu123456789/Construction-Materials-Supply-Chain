using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IAccountingService
    {
        List<LedgerEntryDto> GetLedger(DateTime from, DateTime to);
        List<APAgingDto> GetAPAging();
        List<CashBookDto> GetCashBook(DateTime from, DateTime to);
    }
}
