using Domain;
using Infrastructure.Persistence;
using Infrastructure.Base;
using Infrastructure.Interface;

namespace Infrastructure.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ScmVlxdContext context) : base(context) { }

        public User GetByUsername(string username)
        {
            return _dbSet.FirstOrDefault(u => u.UserName == username);
        }
    }
}
