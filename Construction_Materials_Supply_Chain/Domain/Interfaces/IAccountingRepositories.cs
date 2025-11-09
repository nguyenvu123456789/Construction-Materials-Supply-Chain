using Domain.Interface.Base;
using Domain.Models;
using System.Linq;

namespace Domain.Interfaces
{
    public interface IJournalEntryRepository : IGenericRepository<JournalEntry>
    {
        IQueryable<JournalEntry> QueryFull();
        IQueryable<JournalEntry> QueryByPartner(int partnerId);
        IQueryable<JournalEntry> QueryFullByPartner(int partnerId);
    }

    public interface IAccountRepository : IGenericRepository<GlAccount>
    {
        GlAccount? GetByCode(string code);
        GlAccount? GetByCode(int partnerId, string code);
        IQueryable<GlAccount> QueryByPartner(int partnerId);

        IQueryable<GlAccount> QueryAll(bool includeDeleted = false);
        GlAccount? GetRawById(int id);
        bool ExistsCode(int partnerId, string code, int? excludeId = null);
    }

    public interface ISubLedgerRepository : IGenericRepository<SubLedgerEntry>
    {
        IQueryable<SubLedgerEntry> QueryByPartner(int partnerId, string subType);
        IQueryable<SubLedgerEntry> QueryByInvoice(int partnerId, int invoiceId);
    }

    public interface IPostingPolicyRepository : IGenericRepository<PostingPolicy>
    {
        IQueryable<PostingPolicy> QueryByDoc(string documentType);
        IQueryable<PostingPolicy> QueryByDoc(int partnerId, string documentType);
        PostingPolicy? GetRule(int partnerId, string documentType, string ruleKey);
    }
}
