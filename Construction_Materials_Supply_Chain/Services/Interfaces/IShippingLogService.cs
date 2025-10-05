using BusinessObjects;

namespace Services.Interfaces
{
    public interface IShippingLogService
    {
        List<ShippingLog> GetAll();
        List<ShippingLog> SearchByStatus(string status);
    }
}
