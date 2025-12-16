using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Interfaces
{
    public interface IPaymentService
    {
        void CreatePayment(PaymentDTO dto);
        Payment GetPaymentById(int id);
        List<Payment> GetAllPayments();
        List<Payment> GetPaymentsByPartnerId(int partnerId);
        string GeneratePaymentNumber();
        Task<byte[]> ExportPaymentsToExcelAsync();
        Task ImportPaymentsFromExcelAsync(IFormFile file);
    }
}