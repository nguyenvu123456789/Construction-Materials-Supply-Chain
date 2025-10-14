using Domain.Interface;
using Domain.Models;
using Infrastructure.Repositories.Base;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ScmVlxdContext context) : base(context)
        {
        }

        public Order? GetByCode(string orderCode)
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Material)
                .FirstOrDefault(o => o.OrderCode == orderCode);
        }


        public Order? GetByCodeWithDetails(string orderCode)
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Material)
                .FirstOrDefault(o => o.OrderCode == orderCode);
        }

    }
}
