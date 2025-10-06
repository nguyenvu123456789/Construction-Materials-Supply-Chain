using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportsController : ControllerBase
    {
        private readonly IExportService _service;
        private readonly IMapper _mapper;

        public ExportsController(IExportService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Export>> GetExports()
        {
            var exports = _service.GetAll();
            return Ok(exports);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Export> GetById(int id)
        {
            var exp = _service.GetById(id);
            if (exp == null) return NotFound();
            return Ok(exp);
        }

        [HttpPost]
        public IActionResult CreateExport(ExportDto exportDto)
        {
            var export = _mapper.Map<Export>(exportDto);
            export.Status = string.IsNullOrWhiteSpace(export.Status) ? "Pending" : export.Status;
            export.CreatedAt = export.CreatedAt == default ? DateTime.Now : export.CreatedAt;

            _service.Create(export);
            return CreatedAtAction(nameof(GetById), new { id = export.ExportId }, export);
        }
    }
}
