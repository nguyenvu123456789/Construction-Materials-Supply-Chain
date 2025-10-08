using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportsController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly IMapper _mapper;

        public ExportsController(IExportService exportService, IMapper mapper)
        {
            _exportService = exportService;
            _mapper = mapper;
        }

        // 🔹 Tạo Pending Export
        [HttpPost("pending")]
        public IActionResult CreatePendingExport([FromBody] ExportRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request data.");

            try
            {
                var export = _exportService.CreatePendingExport(dto);
                var result = _mapper.Map<ExportResponseDto>(export);
                return CreatedAtAction(nameof(GetExport), new { id = export.ExportId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Tạo Export thực tế (trừ kho)
        // 🔹 Tạo Export thực tế (trừ kho)
        [HttpPost]
        public IActionResult ConfirmExport([FromBody] ExportConfirmDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.ExportCode))
                return BadRequest("Invalid request data.");

            try
            {
                var export = _exportService.ConfirmExport(dto.ExportCode, dto.Notes);
                var result = _mapper.Map<ExportResponseDto>(export);
                return Ok(result); // 200 OK vì đây là xác nhận, không tạo mới
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // Lấy export theo ID
        [HttpGet("{id}")]
        public IActionResult GetExport(int id)
        {
            var export = _exportService.GetById(id);
            if (export == null) return NotFound();
            var result = _mapper.Map<ExportResponseDto>(export);
            return Ok(result);
        }

        // Lấy danh sách tất cả export
        [HttpGet]
        public IActionResult GetAllExports()
        {
            var exports = _exportService.GetAll();
            var result = _mapper.Map<IEnumerable<ExportResponseDto>>(exports);
            return Ok(result);
        }
    }
}
