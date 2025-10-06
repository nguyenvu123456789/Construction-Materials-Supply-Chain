using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class UserDAO : BaseDAO
    {
        public UserDAO(ScmVlxdContext context) : base(context) { }

        public List<User> GetUsers() => Context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToList();

        public User FindUserById(int userId) =>
            Context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .SingleOrDefault(x => x.UserId == userId);

        public void SaveUser(User u)
        {
            Context.Users.Add(u);
            Context.SaveChanges();
        }

        public void UpdateUser(User u)
        {
            Context.Entry(u).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public void DeleteUserById(int userId)
        {
            var u = Context.Users.SingleOrDefault(x => x.UserId == userId);
            if (u != null)
            {
                Context.Users.Remove(u);
                Context.SaveChanges();
            }
        }

        public List<User> GetUsersPaged(string? searchTerm, List<string>? roles, int pageNumber, int pageSize)
        {
            var query = Context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.UserName.Contains(searchTerm) ||
                                         (u.Email != null && u.Email.Contains(searchTerm)));
            }

            if (roles != null && roles.Any())
            {
                query = query.Where(u => u.UserRoles.Any(ur => roles.Contains(ur.Role.RoleName)));
            }

            return query
                .OrderBy(u => u.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int GetTotalUsersCount(string? searchTerm, List<string>? roles)
        {
            var query = Context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.UserName.Contains(searchTerm) ||
                                         (u.Email != null && u.Email.Contains(searchTerm)));
            }

            if (roles != null && roles.Any())
            {
                query = query.Where(u => u.UserRoles.Any(ur => roles.Contains(ur.Role.RoleName)));
            }

            return query.Count();
        }
    }
}