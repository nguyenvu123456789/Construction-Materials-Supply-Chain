using BusinessObjects;

namespace DataAccess
{
    public class ShippingLogDAO
    {
        public static List<ShippingLog> GetAllShippingLogs()
        {
            using (var context = new ScmVlxdContext())
            {
                return context.ShippingLogs
                              .OrderByDescending(s => s.CreatedAt)
                              .ToList();
            }
        }

        public static List<ShippingLog> SearchShippingLogs(string status)
        {
            using (var context = new ScmVlxdContext())
            {
                return context.ShippingLogs
                              .Where(s => s.Status.Contains(status))
                              .ToList();
            }
        }
    }
}
