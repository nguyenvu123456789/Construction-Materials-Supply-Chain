using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class AccountingEntryRepository : GenericRepository<AccountingEntry>, IAccountingEntryRepository
    {
        public AccountingEntryRepository(ScmVlxdContext context) : base(context)
        {
        }

        public List<AccountingEntry> GetEntries(DateTime from, DateTime to)
        {
            return _dbSet
                .Where(x => x.EntryDate >= from && x.EntryDate <= to)
                .OrderBy(x => x.EntryDate)
                .ToList();
        }
    }
}
