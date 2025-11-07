using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportReportsController : ControllerBase
    {
        private readonly IImportReportService _service;
        private readonly IMapper _mapper;

        public ImportReportsController(IImportReportService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateImportReportDto dto)
        {
            try
            {
                var report = _service.CreateReport(dto);
                var response = _mapper.Map<ImportReportResponseDto>(report);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] int partnerId)
        {
            try
            {
                var reports = _service.GetAllByPartner(partnerId);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var report = _service.GetByIdResponse(id);
                if (report == null)
                    return NotFound(new { message = "Không tìm thấy báo cáo nhập kho." });

                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/view")]
        public IActionResult MarkAsViewed(int id)
        {
            try
            {
                _service.MarkAsViewed(id);
                return Ok(new { message = "Đã đánh dấu báo cáo nhập kho là đã xem." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
