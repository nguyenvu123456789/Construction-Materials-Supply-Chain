using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Interfaces
{
    public interface IPaymentService
    {
        string GeneratePaymentNumber();
        void CreatePayment(PaymentDTO paymentDto, IFormFile file);
        Payment GetPaymentById(int id);
        List<Payment> GetAllPayments();
        List<Payment> GetPaymentsByPartnerId(int partnerId);
    }
}
