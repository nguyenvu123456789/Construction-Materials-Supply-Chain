using API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialChecksController : ControllerBase
    {
        private readonly IMaterialCheckRepository _repository;
        private readonly IMapper _mapper;

        public MaterialChecksController(IMaterialCheckRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/MaterialChecks
        [HttpGet]
        public ActionResult<IEnumerable<MaterialCheckDto>> GetAll()
        {
            var checks = _repository.GetAll();
            return Ok(_mapper.Map<IEnumerable<MaterialCheckDto>>(checks));
        }

        // GET: api/MaterialChecks/5
        [HttpGet("{id}")]
        public ActionResult<MaterialCheckDto> GetById(int id)
        {
            var check = _repository.GetById(id);
            if (check == null) return NotFound();
            return Ok(_mapper.Map<MaterialCheckDto>(check));
        }

        // POST: api/MaterialChecks
        [HttpPost]
        public IActionResult Create(MaterialCheckDto dto)
        {
            var mc = _mapper.Map<MaterialCheck>(dto);
            _repository.Save(mc);
            return NoContent();
        }

        // PUT: api/MaterialChecks/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, MaterialCheckDto dto)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return NotFound();

            var mc = _mapper.Map<MaterialCheck>(dto);
            mc.CheckId = id;
            _repository.Update(mc);

            return NoContent();
        }

        // DELETE: api/MaterialChecks/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return NotFound();

            _repository.Delete(id);
            return NoContent();
        }
    }
}
