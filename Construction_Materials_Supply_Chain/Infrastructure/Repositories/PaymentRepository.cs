using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ScmVlxdContext context) : base(context) { }

        public void AddPaymentInvoice(PaymentInvoice paymentInvoice)
        {
            _context.Set<PaymentInvoice>().Add(paymentInvoice);
            _context.SaveChanges();
        }

        public List<Payment> GetPaymentsByPartnerId(int partnerId)
        {
            return _dbSet.Where(p => p.PartnerId == partnerId).ToList();
        }
    }
}
