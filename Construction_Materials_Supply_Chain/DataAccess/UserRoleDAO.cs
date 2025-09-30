using BusinessObjects;

namespace DataAccess
{
    public class UserRoleDAO : BaseDAO
    {
        public UserRoleDAO(ScmVlxdContext context) : base(context) { }

        public List<UserRole> GetUserRoles() => Context.UserRoles.ToList();

        public void AssignRole(UserRole ur)
        {
            Context.UserRoles.Add(ur);
            Context.SaveChanges();
        }
    }
}
