using BusinessObjects;

namespace Repositories.Interface
{
    public interface IVendorRepository
    {
        List<Vendor> GetApprovedVendors();
        List<Vendor> SearchVendors(string keyword);
    }
}
