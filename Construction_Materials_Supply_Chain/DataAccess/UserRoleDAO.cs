using BusinessObjects;

namespace DataAccess
{
    public class UserRoleDAO
    {
        public static List<UserRole> GetUserRoles()
        {
            var list = new List<UserRole>();
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    list = context.UserRoles.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }

        public static void AssignRole(UserRole ur)
        {
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    context.UserRoles.Add(ur);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
