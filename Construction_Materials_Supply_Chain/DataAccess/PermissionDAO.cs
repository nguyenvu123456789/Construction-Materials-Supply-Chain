using BusinessObjects;

namespace DataAccess
{
    public class PermissionDAO
    {
        public static List<Permission> GetPermissions()
        {
            var list = new List<Permission>();
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    list = context.Permissions.ToList();
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
