using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class JournalEntryRepository : GenericRepository<JournalEntry>, IJournalEntryRepository
    {
        public JournalEntryRepository(ScmVlxdContext context) : base(context) { }
        public IQueryable<JournalEntry> QueryFull() => _dbSet.Include(x => x.Lines).ThenInclude(l => l.Account);
        public IQueryable<JournalEntry> QueryByPartner(int partnerId) => _dbSet.Where(x => x.PartnerId == partnerId);
        public IQueryable<JournalEntry> QueryFullByPartner(int partnerId) => _dbSet.Where(x => x.PartnerId == partnerId).Include(x => x.Lines).ThenInclude(l => l.Account);
    }

    public class AccountRepository : GenericRepository<GlAccount>, IAccountRepository
    {
        public AccountRepository(ScmVlxdContext context) : base(context) { }

        public GlAccount? GetByCode(string code) => _dbSet.FirstOrDefault(a => a.Code == code);
        public GlAccount? GetByCode(int partnerId, string code) => _dbSet.FirstOrDefault(a => a.PartnerId == partnerId && a.Code == code);
        public IQueryable<GlAccount> QueryByPartner(int partnerId) => _dbSet.AsNoTracking().Where(a => a.PartnerId == partnerId);

        public IQueryable<GlAccount> QueryAll(bool includeDeleted = false)
            => includeDeleted ? _context.GlAccounts.IgnoreQueryFilters() : _dbSet.AsNoTracking();

        public GlAccount? GetRawById(int id)
            => _context.GlAccounts.IgnoreQueryFilters().FirstOrDefault(x => x.AccountId == id);

        public bool ExistsCode(int partnerId, string code, int? excludeId = null)
        {
            var q = _context.GlAccounts.IgnoreQueryFilters()
                       .Where(x => x.PartnerId == partnerId && x.Code == code && !x.IsDeleted);
            if (excludeId.HasValue) q = q.Where(x => x.AccountId != excludeId.Value);
            return q.Any();
        }
    }

    public class SubLedgerRepository : GenericRepository<SubLedgerEntry>, ISubLedgerRepository
    {
        public SubLedgerRepository(ScmVlxdContext context) : base(context) { }
        public IQueryable<SubLedgerEntry> QueryByPartner(int partnerId, string subType) =>
            _dbSet.AsNoTracking().Where(x => x.PartnerId == partnerId && x.SubLedgerType == subType);
        public IQueryable<SubLedgerEntry> QueryByInvoice(int partnerId, int invoiceId) =>
            _dbSet.AsNoTracking().Where(x => x.PartnerId == partnerId && x.InvoiceId == invoiceId);
    }

    public class PostingPolicyRepository : GenericRepository<PostingPolicy>, IPostingPolicyRepository
    {
        public PostingPolicyRepository(ScmVlxdContext context) : base(context) { }
        public IQueryable<PostingPolicy> QueryByDoc(string documentType) =>
            _dbSet.AsNoTracking().Where(p => p.DocumentType == documentType);
        public IQueryable<PostingPolicy> QueryByDoc(int partnerId, string documentType) =>
            _dbSet.AsNoTracking().Where(p => p.PartnerId == partnerId && p.DocumentType == documentType);
        public PostingPolicy? GetRule(int partnerId, string documentType, string ruleKey) =>
            _dbSet.AsNoTracking().FirstOrDefault(p => p.PartnerId == partnerId && p.DocumentType == documentType && p.RuleKey == ruleKey);
    }
}
