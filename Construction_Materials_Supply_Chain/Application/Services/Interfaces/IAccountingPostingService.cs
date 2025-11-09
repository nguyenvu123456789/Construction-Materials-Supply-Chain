using Application.DTOs;

namespace Domain.Interfaces
{
    public interface IAccountingPostingService
    {
        PostResultDto PostSalesInvoice(int invoiceId);
        PostResultDto PostSalesInvoiceByCode(string invoiceCode);
        PostResultDto PostPurchaseInvoice(int invoiceId);
        PostResultDto PostReceipt(int receiptId);
        PostResultDto PostPayment(int paymentId);
        PostResultDto Unpost(string sourceType, int sourceId);
    }
}