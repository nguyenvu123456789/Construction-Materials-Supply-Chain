using Application.DTOs;
using System.Collections.Generic;

namespace Application.Services.Interfaces
{
    public interface IReceiptService
    {
        List<ReceiptDTO> GetAllReceipts();
        ReceiptDTO GetReceiptById(int id);
        void AddReceipt(ReceiptDTO receiptDTO);
        void UpdateReceipt(ReceiptDTO receiptDTO);
        void DeleteReceipt(int id);
        List<ReceiptDTO> GetReceiptsByPartnerId(int partnerId);
    }
}
