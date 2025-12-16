using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IAccountingEntryRepository : IGenericRepository<AccountingEntry>
    {
        List<AccountingEntry> GetEntries(DateTime from, DateTime to);
    }
}
