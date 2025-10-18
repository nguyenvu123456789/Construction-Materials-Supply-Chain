using Domain.Models;

namespace Application.Interfaces
{
    public interface IShippingLogService
    {
        List<ShippingLog> GetAll();
    }
}
