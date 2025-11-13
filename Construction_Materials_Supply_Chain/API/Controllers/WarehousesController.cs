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

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var warehouse = _service.GetById(id);
            if (warehouse == null)
                return NotFound(new { message = "Warehouse not found." });

            var result = _mapper.Map<WarehouseDto>(warehouse);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] int? partnerId)
        {
            try
            {
                if (partnerId != null)
                {
                    var warehouses = _service.GetByPartnerId(partnerId.Value);
                    var result = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
                    return Ok(result);
                }

                var allWarehouses = _service.GetAll();
                var allResult = _mapper.Map<IEnumerable<WarehouseDto>>(allWarehouses);
                return Ok(allResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
