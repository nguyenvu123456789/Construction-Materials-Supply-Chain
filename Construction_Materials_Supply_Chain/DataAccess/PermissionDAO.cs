using BusinessObjects;

namespace DataAccess
{
    public class PermissionDAO : BaseDAO
    {
        public PermissionDAO(ScmVlxdContext context) : base(context) { }

        public List<Permission> GetPermissions()
        {
            return Context.Permissions.ToList();
        }
    }
}
