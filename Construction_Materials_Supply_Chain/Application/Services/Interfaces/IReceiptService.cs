using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Interfaces
{
    public interface IReceiptService
    {
        string GenerateReceiptNumber();
        void CreateReceipt(ReceiptDTO receiptDto, IFormFile file);
        Receipt GetReceiptById(int id);
        List<Receipt> GetReceiptsByPartnerId(int partnerId);
        List<Receipt> GetAllReceipts();
    }
}
