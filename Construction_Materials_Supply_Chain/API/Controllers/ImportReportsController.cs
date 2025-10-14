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


        [HttpPost("{id}/review")]
        public IActionResult Review(int id, [FromBody] ReviewImportReportDto dto)
        {
            try
            {
                var report = _service.ReviewReport(id, dto);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pending")]
        public IActionResult GetPending()
        {
            var reports = _service.GetAllPending();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var report = _service.GetById(id);
            if (report == null) return NotFound();
            return Ok(report);
        }
    }
}
