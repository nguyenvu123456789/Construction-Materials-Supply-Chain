using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Payment CreatePayment(Payment payment, int partnerId);
    }
}
