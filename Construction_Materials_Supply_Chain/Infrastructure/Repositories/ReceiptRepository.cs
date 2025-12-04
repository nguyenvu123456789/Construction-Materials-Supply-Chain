using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReceiptRepository : GenericRepository<Receipt>, IReceiptRepository
    {
        public ReceiptRepository(ScmVlxdContext context) : base(context) { }

        public IQueryable<Receipt> QueryReceipts(int partnerId)
        {
            return _dbSet.Where(r => r.PartnerId == partnerId);
        }
    }
}
