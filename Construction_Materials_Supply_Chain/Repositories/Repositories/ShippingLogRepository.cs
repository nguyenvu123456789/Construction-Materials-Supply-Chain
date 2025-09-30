using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class ShippingLogRepository : IShippingLogRepository
    {
        private readonly ShippingLogDAO _dao;

        public ShippingLogRepository(ShippingLogDAO dao)
        {
            _dao = dao;
        }

        public List<ShippingLog> GetAllShippingLogs() => _dao.GetAllShippingLogs();
        public List<ShippingLog> SearchShippingLogs(string status) => _dao.SearchShippingLogs(status);
    }
}
