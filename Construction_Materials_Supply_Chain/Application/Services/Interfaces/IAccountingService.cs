using Application.DTOs;

namespace Application.Services.Interfaces
{
    public interface IAccountingService
    {
        List<LedgerEntryDto> GetLedger(DateTime from, DateTime to, int? partnerId);
        List<APAgingDto> GetAPAging(int? partnerId);
        List<CashBookDto> GetCashBook(DateTime from, DateTime to, int? partnerId);
    }
}
