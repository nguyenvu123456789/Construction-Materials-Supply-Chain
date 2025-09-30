using BusinessObjects;

namespace DataAccess
{
    public class UserDAO
    {
        public static List<User> GetUsers()
        {
            var listUsers = new List<User>();
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    listUsers = context.Users.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listUsers;
        }

        public static User FindUserById(int userId)
        {
            User u = new User();
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    u = context.Users.SingleOrDefault(x => x.UserId == userId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return u;
        }

        public static void SaveUser(User u)
        {
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    context.Users.Add(u);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void UpdateUser(User u)
        {
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    context.Entry<User>(u).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteUser(User u)
        {
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    var u1 = context.Users.SingleOrDefault(x => x.UserId == u.UserId);
                    context.Users.Remove(u1);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static List<User> SearchUsers(string keyword)
        {
            using (var context = new ScmVlxdContext())
            {
                return context.Users
                              .Where(u => u.UserName.Contains(keyword)
                                       || u.Email.Contains(keyword))
                              .ToList();
            }
        }

        public static void DeleteUserById(int userId)
        {
            using (var context = new ScmVlxdContext())
            {
                var u = context.Users.SingleOrDefault(x => x.UserId == userId);
                if (u != null)
                {
                    context.Users.Remove(u);
                    context.SaveChanges();
                }
            }
        }
    }
}
