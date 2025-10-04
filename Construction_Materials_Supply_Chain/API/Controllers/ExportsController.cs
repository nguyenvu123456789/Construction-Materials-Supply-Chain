using API.DTOs;
using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportsController : ControllerBase
    {
        private readonly IExportRepository _exportRepository;
        private readonly IMapper _mapper;

        public ExportsController(IExportRepository exportRepository, IMapper mapper)
        {
            _exportRepository = exportRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Export>> GetExports()
        {
            var exports = _exportRepository.GetExports();
            return Ok(exports);
        }

        [HttpPost]
        public IActionResult CreateExport(ExportDto exportDto)
        {
            var export = _mapper.Map<Export>(exportDto);
            export.Status = "Pending";
            export.CreatedAt = DateTime.Now;

            _exportRepository.SaveExport(export);

            return CreatedAtAction(nameof(GetExports), new { id = export.ExportId }, export);
        }
    }
}
