using BusinessObjects;

namespace DataAccess
{
    public class RoleDAO
    {
        public static List<Role> GetRoles()
        {
            var list = new List<Role>();
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    list = context.Roles.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }
    }
}
