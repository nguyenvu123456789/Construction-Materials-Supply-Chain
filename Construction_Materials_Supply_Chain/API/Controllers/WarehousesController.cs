using Application.Constants.Messages;
using Application.DTOs;
using Application.Interfaces;
using Application.Services.Implements;
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

        // CREATE
        [HttpPost]
        public IActionResult Create([FromBody] WarehouseCreateDto dto)
        {
            try
            {
                var warehouse = _service.Create(dto);
                var result = _mapper.Map<WarehouseDto>(warehouse);

                return CreatedAtAction(nameof(GetById), new { id = result.WarehouseId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // UPDATE
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

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var success = _service.Delete(id);

                if (!success)
                    return NotFound(new { message = WarehouseMessages.MSG_WAREHOUSE_NOT_FOUND });

                return Ok(new { message = WarehouseMessages.MSG_WAREHOUSE_DELETE_SUCCESS });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var warehouse = _service.GetById(id);

            if (warehouse == null)
                return NotFound(new { message = WarehouseMessages.MSG_WAREHOUSE_NOT_FOUND });

            var result = _mapper.Map<WarehouseDto>(warehouse);
            return Ok(result);
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetAll([FromQuery] int? managerId, [FromQuery] int? partnerId)
        {
            try
            {
                var warehouses = _service.GetAll(managerId, partnerId);
                var result = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-warehouses")]
        public IActionResult GetMyWarehouses(int partnerId)
        {
            var result = _service.GetByPartner(partnerId);

            return Ok(result);
        }
    }
}
