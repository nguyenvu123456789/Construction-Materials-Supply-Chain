using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        public List<Vendor> GetApprovedVendors() => VendorDAO.GetApprovedVendors();
        public List<Vendor> SearchVendors(string keyword) => VendorDAO.SearchVendors(keyword);
    }
}
