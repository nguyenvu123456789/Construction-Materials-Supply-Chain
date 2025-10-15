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
    }

    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(ScmVlxdContext context) : base(context) { }
        public Account? GetByCode(string code) => _dbSet.FirstOrDefault(a => a.Code == code);
    }

    public class SubLedgerRepository : GenericRepository<SubLedgerEntry>, ISubLedgerRepository
    {
        public SubLedgerRepository(ScmVlxdContext context) : base(context) { }
        public IQueryable<SubLedgerEntry> QueryByPartner(int partnerId, string subType) =>
            _dbSet.AsNoTracking().Where(x => x.PartnerId == partnerId && x.SubLedgerType == subType);
    }

    public class PostingPolicyRepository : GenericRepository<PostingPolicy>, IPostingPolicyRepository
    {
        public PostingPolicyRepository(ScmVlxdContext context) : base(context) { }
        public IQueryable<PostingPolicy> QueryByDoc(string documentType) =>
            _dbSet.AsNoTracking().Where(p => p.DocumentType == documentType);
    }
}
