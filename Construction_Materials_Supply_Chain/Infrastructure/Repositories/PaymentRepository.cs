using Domain.Interface.Base;
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

        public List<Payment> GetPaymentsByPartnerId(int partnerId)
        {
            return _context.Payments
                .Where(p => p.PartnerId == partnerId)
                .ToList();
        }

        public Payment GetLastPaymentByPrefix(string prefix)
        {
            return _context.Payments
                .AsNoTracking()
                .Where(p => p.PaymentNumber.StartsWith(prefix))
                .OrderByDescending(p => p.PaymentNumber)
                .FirstOrDefault();
        }

        public List<Payment> GetPaymentsByInvoice(string code)
        {
            return _context.Payments
                .Where(x => x.Invoices.Equals(code))
                .ToList();
        }

        public List<Payment> GetByDateRange(DateTime from, DateTime to)
        {
            return _context.Payments
                .Where(x => x.DateCreated >= from && x.DateCreated <= to)
                .OrderBy(x => x.DateCreated)
                .ToList();
        }
    }
}
