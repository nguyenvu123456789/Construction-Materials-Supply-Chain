using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Order? GetByCode(string orderCode);
        Order? GetByCodeWithDetails(string orderCode);
        List<Order> GetAllWithDetails();
        List<Order> GetAllWithWarehouseAndSupplier();
        List<Order> GetSalesOrders(int warehouseId);
    }
}
