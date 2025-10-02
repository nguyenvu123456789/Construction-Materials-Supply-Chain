using API.DTOs;
using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportRequestsController : ControllerBase
    {
        private readonly IExportRequestRepository _repository;
        private readonly IMapper _mapper;
        private readonly ScmVlxdContext _context; // dùng để check stock

        public ExportRequestsController(IExportRequestRepository repository, IMapper mapper, ScmVlxdContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        // GET: api/ExportRequests
        [HttpGet]
        public ActionResult<IEnumerable<ExportRequestDto>> GetAll()
        {
            var exports = _repository.GetAll();
            return Ok(_mapper.Map<IEnumerable<ExportRequestDto>>(exports));
        }

        // GET: api/ExportRequests/5
        [HttpGet("{id}")]
        public ActionResult<ExportRequestDto> GetById(int id)
        {
            var export = _repository.GetById(id);
            if (export == null) return NotFound();

            return Ok(_mapper.Map<ExportRequestDto>(export));
        }

        // POST: api/ExportRequests
        [HttpPost]
        public ActionResult<ExportRequestDto> Create([FromBody] ExportRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var request = _mapper.Map<ExportRequest>(dto);
            var created = _repository.CreateExport(request);
            var result = _mapper.Map<ExportRequestDto>(created);

            return Ok(result);
        }

        // POST: api/ExportRequests/CheckStock
        [HttpPost("CheckStock")]
        public ActionResult<IEnumerable<CheckStockResultDto>> CheckStock([FromBody] CheckStockRequestDto request)
        {
            var results = new List<CheckStockResultDto>();

            foreach (var item in request.Items)
            {
                var inventory = _context.Inventories
                    .FirstOrDefault(i => i.WarehouseId == request.WarehouseId && i.MaterialId == item.MaterialId);

                var available = inventory?.Quantity ?? 0;

                results.Add(new CheckStockResultDto
                {
                    MaterialId = item.MaterialId,
                    Requested = item.Quantity,
                    Available = available,
                    IsEnough = available >= item.Quantity
                });
            }

            return Ok(results);
        }
    }
}
