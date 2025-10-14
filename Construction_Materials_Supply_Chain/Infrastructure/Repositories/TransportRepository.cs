using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class TransportRepository : GenericRepository<Transport>, ITransportRepository
    {
        public TransportRepository(ScmVlxdContext context) : base(context)
        {
        }

        public Transport? GetById(int id)
        {
            return _dbSet.Find(id);
        }
    }
}
