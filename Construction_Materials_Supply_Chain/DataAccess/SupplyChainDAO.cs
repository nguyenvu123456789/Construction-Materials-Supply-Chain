using BusinessObjects;

namespace DataAccess
{
    public class SupplyChainDAO
    {
        public static List<Warehouse> GetWarehouses()
        {
            using (var context = new ScmVlxdContext())
            {
                return context.Warehouses.ToList();
            }
        }

        public static List<Supplier> GetSuppliers()
        {
            using (var context = new ScmVlxdContext())
            {
                return context.Suppliers.ToList();
            }
        }

        public static List<Transport> GetTransports()
        {
            using (var context = new ScmVlxdContext())
            {
                return context.Transports.ToList();
            }
        }

        public static void UpdateWarehouse(Warehouse w)
        {
            using (var context = new ScmVlxdContext())
            {
                context.Entry(w).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void DeleteWarehouse(int id)
        {
            using (var context = new ScmVlxdContext())
            {
                var w = context.Warehouses.SingleOrDefault(x => x.WarehouseId == id);
                if (w != null)
                {
                    context.Warehouses.Remove(w);
                    context.SaveChanges();
                }
            }
        }
    }
}
