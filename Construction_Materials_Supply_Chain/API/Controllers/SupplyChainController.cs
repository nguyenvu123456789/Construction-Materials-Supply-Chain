//using API.DTOs;
//using AutoMapper;
//using BusinessObjects;
//using Microsoft.AspNetCore.Mvc;
//using Repositories.Interface;

//namespace API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SupplyChainController : ControllerBase
//    {
//        private readonly ISupplyChainRepository _repository;
//        private readonly IMapper _mapper;

//        public SupplyChainController(ISupplyChainRepository repository, IMapper mapper)
//        {
//            _repository = repository;
//            _mapper = mapper;
//        }

//        [HttpGet("Warehouses")]
//        public ActionResult<IEnumerable<WarehouseDto>> GetWarehouses()
//        {
//            var warehouses = _repository.GetWarehouses();
//            return Ok(_mapper.Map<IEnumerable<WarehouseDto>>(warehouses));
//        }

//        [HttpGet("Suppliers")]
//        public ActionResult<IEnumerable<SupplierDto>> GetSuppliers()
//        {
//            var suppliers = _repository.GetSuppliers();
//            return Ok(_mapper.Map<IEnumerable<SupplierDto>>(suppliers));
//        }

//        [HttpGet("Transports")]
//        public ActionResult<IEnumerable<TransportDto>> GetTransports()
//        {
//            var transports = _repository.GetTransports();
//            return Ok(_mapper.Map<IEnumerable<TransportDto>>(transports));
//        }

//        [HttpPut("Warehouse/{id}")]
//        public IActionResult UpdateWarehouse(int id, WarehouseDto warehouseDto)
//        {
//            var warehouse = _mapper.Map<Warehouse>(warehouseDto);
//            warehouse.WarehouseId = id;
//            _repository.UpdateWarehouse(warehouse);
//            return NoContent();
//        }

//        [HttpDelete("Warehouse/{id}")]
//        public IActionResult DeleteWarehouse(int id)
//        {
//            _repository.DeleteWarehouse(id);
//            return NoContent();
//        }
//    }
//}
