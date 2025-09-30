using BusinessObjects;

namespace DataAccess
{
    public class RoleDAO : BaseDAO
    {
        public RoleDAO(ScmVlxdContext context) : base(context) { }

        public List<Role> GetRoles()
        {
            return Context.Roles.ToList();
        }
    }
}
