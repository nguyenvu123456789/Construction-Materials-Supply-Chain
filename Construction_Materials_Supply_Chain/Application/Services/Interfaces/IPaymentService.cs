using Application.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

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