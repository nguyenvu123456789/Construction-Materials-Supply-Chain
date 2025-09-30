using BusinessObjects;
using DataAccess;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class VendorRepository : IVendorRepository
    {
        public List<Vendor> GetApprovedVendors() => VendorDAO.GetApprovedVendors();
        public List<Vendor> SearchVendors(string keyword) => VendorDAO.SearchVendors(keyword);
    }
}
