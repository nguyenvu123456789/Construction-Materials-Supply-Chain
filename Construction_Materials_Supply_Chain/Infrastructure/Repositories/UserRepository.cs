using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ScmVlxdContext context) : base(context) { }

        public User GetByUsername(string username)
        {
            return _dbSet.FirstOrDefault(u => u.UserName == username);
        }

        public bool ExistsByUsername(string username)
        {
            return _dbSet.Any(u => u.UserName == username);
        }

        public IQueryable<User> QueryWithRoles()
        => _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);
    }
}
