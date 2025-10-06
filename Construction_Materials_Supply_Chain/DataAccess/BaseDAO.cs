using BusinessObjects;

namespace DataAccess
{
    public class BaseDAO
    {
        protected readonly ScmVlxdContext Context;

        public BaseDAO(ScmVlxdContext context)
        {
            Context = context;
        }
    }
}
