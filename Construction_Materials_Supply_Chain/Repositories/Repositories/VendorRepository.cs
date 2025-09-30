using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly VendorDAO _dao;

        public VendorRepository(VendorDAO dao)
        {
            _dao = dao;
        }

        public List<Vendor> GetApprovedVendors() => _dao.GetApprovedVendors();
        public List<Vendor> SearchVendors(string keyword) => _dao.SearchVendors(keyword);
    }
}
