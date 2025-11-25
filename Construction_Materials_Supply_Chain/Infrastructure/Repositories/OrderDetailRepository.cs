using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly ScmVlxdContext _context;

        public OrderDetailRepository(ScmVlxdContext context)
        {
            _context = context;
        }

        public List<OrderDetail> GetAll()
        {
            return _context.OrderDetails
                .Include(od => od.Material)
                .Include(od => od.Order)
                .ToList();
        }

        public List<OrderDetail> GetByOrderId(int orderId)
        {
            return _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Material)
                .ToList();
        }
        public OrderDetail? GetByOrderAndMaterial(int orderId, int materialId)
        {
            return _context.OrderDetails
                .AsNoTracking()
                .Include(od => od.Material)
                .Include(od => od.Order)
                .FirstOrDefault(od => od.OrderId == orderId && od.MaterialId == materialId);
        }

        public List<OrderDetail> GetByMaterialIds(List<int> materialIds)
        {
            return _context.OrderDetails
                .Where(od => materialIds.Contains(od.MaterialId))
                .Include(od => od.Material)
                .Include(od => od.Order)
                .ToList();
        }

        public OrderDetail? GetById(int id)
        {
            return _context.OrderDetails
                .Include(od => od.Material)
                .Include(od => od.Order)
                .FirstOrDefault(od => od.OrderDetailId == id);
        }

        public void Add(OrderDetail detail)
        {
            _context.OrderDetails.Add(detail);
        }

        public void Update(OrderDetail detail)
        {
            _context.OrderDetails.Update(detail);
        }

        public void Delete(int id)
        {
            var detail = _context.OrderDetails.Find(id);
            if (detail != null)
            {
                _context.OrderDetails.Remove(detail);
            }
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
