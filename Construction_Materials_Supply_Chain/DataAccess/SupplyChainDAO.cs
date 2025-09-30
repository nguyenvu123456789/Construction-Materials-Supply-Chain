using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class SupplyChainDAO : BaseDAO
    {
        public SupplyChainDAO(ScmVlxdContext context) : base(context) { }

        public List<Warehouse> GetWarehouses() => Context.Warehouses.ToList();
        public List<Supplier> GetSuppliers() => Context.Suppliers.ToList();
        public List<Transport> GetTransports() => Context.Transports.ToList();

        public void UpdateWarehouse(Warehouse w)
        {
            Context.Entry(w).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public void DeleteWarehouse(int id)
        {
            var w = Context.Warehouses.SingleOrDefault(x => x.WarehouseId == id);
            if (w != null)
            {
                Context.Warehouses.Remove(w);
                Context.SaveChanges();
            }
        }
    }
}
