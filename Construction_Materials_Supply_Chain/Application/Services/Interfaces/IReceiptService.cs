using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Interfaces
{
    public interface IReceiptService
    {
        void CreateReceipt(ReceiptDTO dto);
        Receipt GetReceiptById(int id);
        List<Receipt> GetAllReceipts();
        List<Receipt> GetReceiptsByPartnerId(int partnerId);
        string GenerateReceiptNumber();
        Task<byte[]> ExportReceiptsToExcelAsync();
        Task ImportReceiptsFromExcelAsync(IFormFile file);
    }
}