using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAccountingQueryService
    {
        GeneralLedgerResponseDto GetGeneralLedger(DateTime from, DateTime to, string accountCode);
        AgingResponseDto GetARAging(DateTime asOf);
        AgingResponseDto GetAPAging(DateTime asOf);
        CashbookResponseDto GetCashbook(DateTime from, DateTime to, string? method);
        BankReconResponseDto GetBankReconciliation(int statementId);
    }
}
