using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReceiptRepository : GenericRepository<Receipt>, IReceiptRepository
    {
        public ReceiptRepository(ScmVlxdContext context) : base(context) { }

        public List<Receipt> GetReceiptsByPartnerId(int partnerId)
        {
            return _context.Receipts.Where(r => r.PartnerId == partnerId).ToList();
        }

        public Receipt GetLastReceiptByPrefix(string prefix)
        {
            return _context.Receipts
                .AsNoTracking()
                .Where(r => r.ReceiptNumber.StartsWith(prefix))
                .OrderByDescending(r => r.ReceiptNumber)
                .FirstOrDefault();
        }

        public List<Receipt> GetByDateRange(DateTime from, DateTime to)
        {
            return _context.Receipts
                .Where(x => x.DateCreated >= from && x.DateCreated <= to)
                .OrderBy(x => x.DateCreated)
                .ToList();
        }

        public List<Receipt> GetByDateRangeAndPartner(DateTime from, DateTime to, int? partnerId)
        {
            var query = _context.Receipts
                .Where(x => x.DateCreated >= from && x.DateCreated <= to);

            if (partnerId.HasValue)
            {
                query = query.Where(x => x.PartnerId == partnerId.Value);
            }

            return query.OrderBy(x => x.DateCreated).ToList();
        }
    }
}