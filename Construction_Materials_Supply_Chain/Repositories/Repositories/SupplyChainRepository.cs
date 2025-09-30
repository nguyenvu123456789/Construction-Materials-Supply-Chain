using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class SupplyChainRepository : ISupplyChainRepository
    {
        public List<Warehouse> GetWarehouses() => SupplyChainDAO.GetWarehouses();
        public List<Supplier> GetSuppliers() => SupplyChainDAO.GetSuppliers();
        public List<Transport> GetTransports() => SupplyChainDAO.GetTransports();

        public void UpdateWarehouse(Warehouse w) => SupplyChainDAO.UpdateWarehouse(w);
        public void DeleteWarehouse(int id) => SupplyChainDAO.DeleteWarehouse(id);
    }
}
