//using Application.DTOs;
//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Application.Interfaces;

//namespace API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MaterialChecksController : ControllerBase
//    {
//        private readonly IMaterialCheckService _service;
//        private readonly IMapper _mapper;

//        public MaterialChecksController(IMaterialCheckService service, IMapper mapper)
//        {
//            _service = service;
//            _mapper = mapper;
//        }

//        // GET: api/MaterialChecks
//        [HttpGet]
//        public ActionResult<IEnumerable<MaterialCheckDto>> GetAll()
//        {
//            var checks = _service.GetAll();
//            return Ok(_mapper.Map<IEnumerable<MaterialCheckDto>>(checks));
//        }

//        // GET: api/MaterialChecks/5
//        [HttpGet("{id:int}")]
//        public ActionResult<MaterialCheckDto> GetById(int id)
//        {
//            var check = _service.GetById(id);
//            if (check == null) return NotFound();
//            return Ok(_mapper.Map<MaterialCheckDto>(check));
//        }

//        // POST: api/MaterialChecks
//        [HttpPost]
//        public IActionResult Create(MaterialCheckDto dto)
//        {
//            var entity = _mapper.Map<BusinessObjects.MaterialCheck>(dto);
//            _service.Create(entity);
//            return CreatedAtAction(nameof(GetById), new { id = entity.CheckId }, _mapper.Map<MaterialCheckDto>(entity));
//        }

//        // PUT: api/MaterialChecks/5
//        [HttpPut("{id:int}")]
//        public IActionResult Update(int id, MaterialCheckDto dto)
//        {
//            var existing = _service.GetById(id);
//            if (existing == null) return NotFound();

//            var entity = _mapper.Map<BusinessObjects.MaterialCheck>(dto);
//            entity.CheckId = id;
//            _service.Update(entity);

//            return NoContent();
//        }

//        // DELETE: api/MaterialChecks/5
//        [HttpDelete("{id:int}")]
//        public IActionResult Delete(int id)
//        {
//            var existing = _service.GetById(id);
//            if (existing == null) return NotFound();

//            _service.Delete(id);
//            return NoContent();
//        }
//    }
//}
