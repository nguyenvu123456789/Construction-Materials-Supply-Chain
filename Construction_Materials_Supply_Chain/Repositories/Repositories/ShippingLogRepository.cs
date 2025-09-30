using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class ShippingLogRepository : IShippingLogRepository
    {
        public List<ShippingLog> GetAllShippingLogs() => ShippingLogDAO.GetAllShippingLogs();
        public List<ShippingLog> SearchShippingLogs(string status) => ShippingLogDAO.SearchShippingLogs(status);
    }
}
