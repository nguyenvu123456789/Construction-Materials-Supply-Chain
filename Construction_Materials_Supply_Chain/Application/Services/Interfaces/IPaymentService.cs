using Application.DTOs;
using System.Collections.Generic;

namespace Application.Services.Interfaces
{
    public interface IPaymentService
    {
        List<PaymentDTO> GetAllPayments();
        PaymentDTO GetPaymentById(int id);
        void AddPayment(PaymentDTO paymentDTO);
        void UpdatePayment(PaymentDTO paymentDTO);
        void DeletePayment(int id);
        List<PaymentDTO> GetPaymentsByPartnerId(int partnerId);
    }
}
