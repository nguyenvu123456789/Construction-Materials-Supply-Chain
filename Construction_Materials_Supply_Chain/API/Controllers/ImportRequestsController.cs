using API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportRequestsController : ControllerBase
    {
        private readonly IImportRequestRepository _repository;
        private readonly IMapper _mapper;

        public ImportRequestsController(IImportRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/ImportRequests
        [HttpGet]
        public ActionResult<IEnumerable<ImportRequestDto>> GetAll()
        {
            var imports = _repository.GetAll();
            return Ok(_mapper.Map<IEnumerable<ImportRequestDto>>(imports));
        }

        // GET: api/ImportRequests/5
        [HttpGet("{id}")]
        public ActionResult<ImportRequestDto> GetById(int id)
        {
            var import = _repository.GetById(id);
            if (import == null) return NotFound();

            return Ok(_mapper.Map<ImportRequestDto>(import));
        }

        // POST: api/ImportRequests
        [HttpPost]
        public ActionResult<ImportRequestDto> Create([FromBody] ImportRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var request = _mapper.Map<ImportRequest>(dto);
            var created = _repository.CreateImport(request);
            var result = _mapper.Map<ImportRequestDto>(created);

            return Ok(result);
        }
    }
}
