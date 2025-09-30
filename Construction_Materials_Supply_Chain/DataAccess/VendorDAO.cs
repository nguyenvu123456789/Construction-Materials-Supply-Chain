using BusinessObjects;

namespace DataAccess
{
    public class VendorDAO
    {
        public static List<Vendor> GetApprovedVendors()
        {
            using (var context = new ScmVlxdContext())
            {
                return context.Vendors
                              .Where(v => v.Status == "Approved")
                              .ToList();
            }
        }

        public static List<Vendor> SearchVendors(string keyword)
        {
            using (var context = new ScmVlxdContext())
            {
                return context.Vendors
                              .Where(v => v.VendorName.Contains(keyword)
                                       || v.ContactEmail.Contains(keyword))
                              .ToList();
            }
        }
    }
}
