using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VietnamGeoController : ControllerBase
    {
        private readonly IVietnamGeoService _geo;

        public VietnamGeoController(IVietnamGeoService geo)
        {
            _geo = geo;
        }

        // Check district in province
        [HttpGet("check-region")]
        public IActionResult Check(string province, string district)
        {
            bool ok = _geo.IsDistrictInProvince(province, district);
            return Ok(new
            {
                province,
                district,
                isValid = ok
            });
        }

        // Get all provinces
        [HttpGet("provinces")]
        public IActionResult GetProvinces()
        {
            var provinces = _geo.GetProvinces();
            return Ok(provinces);
        }

        // Get districts of a province
        [HttpGet("districts")]
        public IActionResult GetDistricts(string province)
        {
            var districts = _geo.GetDistricts(province);
            return Ok(districts);
        }
    }

}
