using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ScmVlxdContext _context;

        public WarehouseRepository(ScmVlxdContext context)
        {
            _context = context;
        }

        public List<Warehouse> GetAll() => _context.Warehouses.ToList();

        public Warehouse? GetById(int id) => _context.Warehouses.FirstOrDefault(w => w.WarehouseId == id);

        public void Add(Warehouse warehouse)
        {
            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();
        }

        public void Update(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            _context.SaveChanges();
        }

        public void Delete(Warehouse warehouse)
        {
            _context.Warehouses.Remove(warehouse);
            _context.SaveChanges();
        }
        public List<Warehouse> GetByPartnerId(int partnerId)
        {
            return _context.Warehouses
                .Include(w => w.Manager)
                .Where(w => w.Manager != null && w.Manager.PartnerId == partnerId)
                .ToList();
        }
    }
}
