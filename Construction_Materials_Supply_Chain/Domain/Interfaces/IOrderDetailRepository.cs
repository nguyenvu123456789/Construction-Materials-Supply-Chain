using Domain.Models;
using System.Collections.Generic;

namespace Domain.Interface
{
    public interface IOrderDetailRepository
    {
        List<OrderDetail> GetAll();
        List<OrderDetail> GetByOrderId(int orderId);
        OrderDetail? GetById(int id);
        void Add(OrderDetail detail);
        void Update(OrderDetail detail);
        void Delete(int id);
    }
}
