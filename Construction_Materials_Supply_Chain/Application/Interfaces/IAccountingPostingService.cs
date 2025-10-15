using System;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IAccountingPostingService
    {
        void PostSalesInvoice(int invoiceId);
        void PostPurchaseInvoice(int invoiceId);
        void PostExportCogs(int exportId);
        void PostReceipt(int receiptId);
        void PostPayment(int paymentId);
        void Unpost(string sourceType, int sourceId);
    }
}
