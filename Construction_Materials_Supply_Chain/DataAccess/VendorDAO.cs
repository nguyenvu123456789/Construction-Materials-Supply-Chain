using BusinessObjects;

namespace DataAccess
{
    public class VendorDAO : BaseDAO
    {
        public VendorDAO(ScmVlxdContext context) : base(context) { }

        public List<Vendor> GetApprovedVendors() =>
            Context.Vendors.Where(v => v.Status == "Approved").ToList();

        public List<Vendor> SearchVendors(string keyword) =>
            Context.Vendors
                   .Where(v => v.VendorName.Contains(keyword) || v.ContactEmail.Contains(keyword))
                   .ToList();
    }
}
