using Domain;

namespace Application.Interfaces
{
    public interface IShippingLogService
    {
        List<ShippingLog> GetAll();
        List<ShippingLog> SearchByStatus(string status);
    }
}
