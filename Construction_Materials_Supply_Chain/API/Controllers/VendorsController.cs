using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using Repositories.Repositories;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorRepository repository = new VendorRepository();

        // GET: api/Vendors
        [HttpGet]
        public ActionResult<IEnumerable<Vendor>> GetApprovedVendors() => repository.GetApprovedVendors();

        // GET: api/Vendors/Search?keyword=abc
        [HttpGet("Search")]
        public ActionResult<IEnumerable<Vendor>> Search(string keyword) => repository.SearchVendors(keyword);
    }
}
