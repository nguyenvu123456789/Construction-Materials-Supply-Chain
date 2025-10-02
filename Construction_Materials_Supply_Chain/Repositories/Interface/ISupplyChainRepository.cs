using BusinessObjects;

namespace Repositories.Interface
{
    public interface ISupplyChainRepository
    {
        List<Partner> GetPartners();
        List<PartnerType> GetPartnerTypes();
        List<Warehouse> GetWarehouses();
        List<Transport> GetTransports();

        void UpdateWarehouse(Warehouse w);
        void DeleteWarehouse(int id);
    }
}
