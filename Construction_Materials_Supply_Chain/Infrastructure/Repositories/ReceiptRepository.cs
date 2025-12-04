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
    }
}
