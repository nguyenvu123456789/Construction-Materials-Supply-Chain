using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class SupplyChainRepository : ISupplyChainRepository
    {
        private readonly SupplyChainDAO _dao;

        public SupplyChainRepository(SupplyChainDAO dao)
        {
            _dao = dao;
        }

        public List<Warehouse> GetWarehouses() => _dao.GetWarehouses();
        public List<Supplier> GetSuppliers() => _dao.GetSuppliers();
        public List<Transport> GetTransports() => _dao.GetTransports();

        public void UpdateWarehouse(Warehouse w) => _dao.UpdateWarehouse(w);
        public void DeleteWarehouse(int id) => _dao.DeleteWarehouse(id);
    }
}
