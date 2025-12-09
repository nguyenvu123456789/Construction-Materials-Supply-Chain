using Application.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

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