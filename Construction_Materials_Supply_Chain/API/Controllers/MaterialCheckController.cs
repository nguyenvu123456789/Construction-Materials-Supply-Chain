using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Application.DTOs;
using Application.DTOs.Common.Pagination;
using Application.Interfaces;
using Application.Responses;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/material-check")]
    public class MaterialCheckController : ControllerBase
    {
        private readonly IMaterialCheckService _service;
        public MaterialCheckController(IMaterialCheckService service) { _service = service; }

        [HttpPost]
        public ActionResult<ApiResponse<MaterialCheck>> Create([FromBody] MaterialCheckCreateDto dto)
        {
            var result = _service.CreateMaterialCheck(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("handle")]
        public ActionResult<ApiResponse<MaterialCheckHandleResponseDto>> Handle([FromBody] MaterialCheckHandleDto dto)
        {
            var result = _service.HandleMaterialCheck(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public IActionResult GetAll(
    [FromQuery] int? partnerId = null,
    [FromQuery] int? userId = null,
    [FromQuery] string? searchTerm = null,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = _service.GetAllMaterialChecks(partnerId, userId, searchTerm, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{checkId:int}")]
        public IActionResult GetMaterialCheckById(int checkId)
        {
            var response = _service.GetMaterialCheckById(checkId);
            return Ok(response);
        }
    }
}
