using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAccountingQueryService
    {
        GeneralLedgerResponseDto GetGeneralLedger(DateTime from, DateTime to, string accountCode);
        AgingResponseDto GetARAging(DateTime asOf, int? partnerId = null);
        AgingResponseDto GetAPAging(DateTime asOf, int? partnerId = null);
        CashbookResponseDto GetCashbook(DateTime from, DateTime to, string? method, int? partnerId = null);
        BankReconResponseDto GetBankReconciliation(int statementId);
    }
}
