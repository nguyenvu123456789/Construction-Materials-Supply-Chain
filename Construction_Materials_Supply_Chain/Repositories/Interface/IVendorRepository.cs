using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IVendorRepository
    {
        List<Vendor> GetApprovedVendors();
        List<Vendor> SearchVendors(string keyword);
    }
}
