    using Domain.Interface;
    using Domain.Models;
    using Infrastructure.Persistence;
    using Infrastructure.Repositories.Base;
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
                    .Include(o => o.Warehouse)
                    .Include(o => o.OrderDetails)
                    .Include(o => o.CreatedByNavigation)
                        .ThenInclude(u => u.Partner)
                    .FirstOrDefault(o => o.OrderCode == orderCode);
            }

            public Order? GetByCodeWithDetails(string orderCode)
            {
                return _context.Orders
                    .Include(o => o.Warehouse)
                    .Include(o => o.Warehouse)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Material)
                    .FirstOrDefault(o => o.OrderCode == orderCode);
            }
            public List<Order> GetAllWithDetails()
            {
                return _dbSet
                    .Include(o => o.Warehouse)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Material)
                    .Include(o => o.CreatedByNavigation)
                        .ThenInclude(u => u.Partner)
                    .Include(o => o.Supplier)
                    .ToList();
            }
        public List<Order> GetAllWithWarehouseAndSupplier()
        {
            return _context.Orders
                .Include(o => o.Warehouse)
                .Include(o => o.Supplier)
                .Include(o => o.CreatedByNavigation)
                .ThenInclude(u => u.Partner)
                .ThenInclude(u => u.Partner)
                .Include(o => o.OrderDetails)
                .ToList();
        }

        public List<Order> GetSalesOrders(int warehouseId)
        {
            return _context.Orders
                .Include(o => o.Warehouse)
                .Include(o => o.Supplier)
                .Include(o => o.CreatedByNavigation)
                .Include(o => o.OrderDetails)   
                .Where(o => o.WarehouseId == warehouseId)
                .ToList();
        }

    }
    }
