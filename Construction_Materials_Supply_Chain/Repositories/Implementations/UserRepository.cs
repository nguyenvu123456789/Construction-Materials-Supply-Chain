using BusinessObjects;
using DataAccess;
using Repositories.Base;
using Repositories.Interface;

namespace Repositories.Implementations
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
