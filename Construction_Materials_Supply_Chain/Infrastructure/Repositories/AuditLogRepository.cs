using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(ScmVlxdContext context) : base(context) { }

        public IQueryable<AuditLog> QueryWithUser()
            => _dbSet.Include(a => a.User);
    }
}
