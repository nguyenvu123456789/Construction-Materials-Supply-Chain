using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ScmVlxdContext context) : base(context) { }

        public IQueryable<Payment> QueryPayments(int partnerId)
        {
            return _dbSet.Where(p => p.PartnerId == partnerId);
        }
    }
}
