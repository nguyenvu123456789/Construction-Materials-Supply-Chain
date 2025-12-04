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
        public ReceiptRepository(ScmVlxdContext context) : base(context)
        {
        }

        public List<Receipt> GetReceiptsByPartnerId(int partnerId)
        {
            return _dbSet.Where(r => r.PartnerId == partnerId).ToList();
        }
    }
}
