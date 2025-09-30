using BusinessObjects;

namespace DataAccess
{
    public class ShippingLogDAO : BaseDAO
    {
        public ShippingLogDAO(ScmVlxdContext context) : base(context) { }

        public List<ShippingLog> GetAllShippingLogs()
        {
            return Context.ShippingLogs
                          .OrderByDescending(s => s.CreatedAt)
                          .ToList();
        }

        public List<ShippingLog> SearchShippingLogs(string status)
        {
            return Context.ShippingLogs
                          .Where(s => s.Status.Contains(status))
                          .ToList();
        }
    }
}
