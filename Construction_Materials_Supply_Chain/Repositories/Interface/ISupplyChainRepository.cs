using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface ISupplyChainRepository
    {
        List<Warehouse> GetWarehouses();
        List<Supplier> GetSuppliers();
        List<Transport> GetTransports();

        void UpdateWarehouse(Warehouse w);
        void DeleteWarehouse(int id);
    }
}
