using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _service;
        private readonly IMapper _mapper;

        public WarehousesController(IWarehouseService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // ➕ Tạo mới kho
        [HttpPost]
        public IActionResult Create([FromBody] WarehouseCreateDto dto)
        {
            try
            {
                var warehouse = _service.Create(dto);
                var result = _mapper.Map<WarehouseDto>(warehouse);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✏️ Cập nhật kho
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] WarehouseUpdateDto dto)
        {
            try
            {
                var warehouse = _service.Update(id, dto);
                var result = _mapper.Map<WarehouseDto>(warehouse);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ❌ Xóa kho
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var success = _service.Delete(id);
                if (!success)
                    return NotFound(new { message = "Warehouse not found." });

                return Ok(new { message = "Warehouse deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 📋 Lấy tất cả kho
        [HttpGet]
        public IActionResult GetAll()
        {
            var warehouses = _service.GetAll();
            var result = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
            return Ok(result);
        }

        // 🔍 Lấy kho theo ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var warehouse = _service.GetById(id);
            if (warehouse == null)
                return NotFound(new { message = "Warehouse not found." });

            var result = _mapper.Map<WarehouseDto>(warehouse);
            return Ok(result);
        }
    }
}
