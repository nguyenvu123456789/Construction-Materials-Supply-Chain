using Domain.Interface.Base;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IJournalEntryRepository : IGenericRepository<JournalEntry>
    {
        IQueryable<JournalEntry> QueryFull();
    }

    public interface IAccountRepository : IGenericRepository<Account>
    {
        Account? GetByCode(string code);
    }

    public interface ISubLedgerRepository : IGenericRepository<SubLedgerEntry>
    {
        IQueryable<SubLedgerEntry> QueryByPartner(int partnerId, string subType);
    }

    public interface IPostingPolicyRepository : IGenericRepository<PostingPolicy>
    {
        IQueryable<PostingPolicy> QueryByDoc(string documentType);
    }
}
