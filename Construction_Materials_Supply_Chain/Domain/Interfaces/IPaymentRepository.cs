using Domain.Interface.Base;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        List<Payment> GetPaymentsByPartnerId(int partnerId);
        Payment GetLastPaymentByPrefix(string prefix);
        List<Payment> GetByDateRange(DateTime from, DateTime to);
        List<Payment> GetPaymentsByInvoice(string code);
        List<Payment> GetByDateRangeAndPartner(DateTime from, DateTime to, int? partnerId);
    }
}
