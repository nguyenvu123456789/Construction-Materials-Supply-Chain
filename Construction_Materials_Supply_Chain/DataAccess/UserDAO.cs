using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class UserDAO : BaseDAO
    {
        public UserDAO(ScmVlxdContext context) : base(context) { }

        public List<User> GetUsers() => Context.Users.ToList();

        public User FindUserById(int userId) =>
            Context.Users.SingleOrDefault(x => x.UserId == userId);

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

        public void DeleteUser(User u)
        {
            var u1 = Context.Users.SingleOrDefault(x => x.UserId == u.UserId);
            if (u1 != null)
            {
                Context.Users.Remove(u1);
                Context.SaveChanges();
            }
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

        public List<User> GetUsersPaged(string? keyword, int pageNumber, int pageSize)
        {
            var query = Context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.UserName.Contains(keyword) || u.Email.Contains(keyword));
            }
            return query
                .OrderBy(u => u.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int GetTotalUsersCount(string? keyword)
        {
            var query = Context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.UserName.Contains(keyword) || u.Email.Contains(keyword));
            }
            return query.Count();
        }
    }
}
