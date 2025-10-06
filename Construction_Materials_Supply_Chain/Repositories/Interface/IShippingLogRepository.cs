using BusinessObjects;

namespace Repositories.Interface
{
    public interface IShippingLogRepository
    {
        List<ShippingLog> GetAllShippingLogs();
        List<ShippingLog> SearchShippingLogs(string status);
    }
}
