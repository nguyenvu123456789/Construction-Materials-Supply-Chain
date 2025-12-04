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
    public class AccountingEntryRepository : GenericRepository<AccountingEntry>, IAccountingEntryRepository
    {
        public AccountingEntryRepository(ScmVlxdContext context) : base(context)
        {
        }

        public List<AccountingEntry> GetEntriesByDocumentType(string documentType)
        {
            return _dbSet.Where(a => a.DocumentType == documentType).ToList();
        }
    }
}
