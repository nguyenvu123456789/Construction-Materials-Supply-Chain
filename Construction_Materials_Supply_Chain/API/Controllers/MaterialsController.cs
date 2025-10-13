using Application.Common.Pagination;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        private readonly IMapper _mapper;

        public MaterialsController(IMaterialService materialService, IMapper mapper)
        {
            _materialService = materialService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<MaterialDto>> GetMaterials()
        {
            var materials = _materialService.GetAll();
            var result = _mapper.Map<IEnumerable<MaterialDto>>(materials);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<MaterialDto> GetMaterial(int id)
        {
            var material = _materialService.GetById(id);
            if (material == null) return NotFound();
            return Ok(_mapper.Map<MaterialDto>(material));
        }

        [HttpPost]
        public IActionResult CreateMaterial(MaterialDto dto)
        {
            var material = _mapper.Map<Material>(dto);
            _materialService.Create(material);
            return CreatedAtAction(nameof(GetMaterial), new { id = material.MaterialId }, _mapper.Map<MaterialDto>(material));
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateMaterial(int id, MaterialDto dto)
        {
            var existing = _materialService.GetById(id);
            if (existing == null) return NotFound();

            var material = _mapper.Map<Material>(dto);
            material.MaterialId = id;
            _materialService.Update(material);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteMaterial(int id)
        {
            var existing = _materialService.GetById(id);
            if (existing == null) return NotFound();

            _materialService.Delete(id);
            return NoContent();
        }

        // GET: api/materials/filter?SearchTerm=...&PageNumber=1&PageSize=10
        [HttpGet("filter")]
        public ActionResult<PagedResultDto<MaterialDto>> GetMaterialsFiltered([FromQuery] PagedQueryDto queryParams)
        {
            var materials = _materialService.GetMaterialsFiltered(
                queryParams.SearchTerm,
                queryParams.PageNumber,
                queryParams.PageSize,
                out var totalCount
            );

            var result = new PagedResultDto<MaterialDto>
            {
                Data = _mapper.Map<IEnumerable<MaterialDto>>(materials),
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize)
            };

            return Ok(result);
        }
        // GET: api/materials/by-category/{categoryId}
        [HttpGet("by-category/{categoryId:int}")]
        public ActionResult<IEnumerable<MaterialDto>> GetByCategory(int categoryId)
        {
            var materials = _materialService.GetByCategory(categoryId);
            if (materials == null || !materials.Any())
                return NotFound("No materials found for this category.");

            var result = _mapper.Map<IEnumerable<MaterialDto>>(materials);
            return Ok(result);
        }
        // GET: api/materials/by-warehouse/{warehouseId}
        [HttpGet("by-warehouse/{warehouseId:int}")]
        public ActionResult<IEnumerable<MaterialDto>> GetByWarehouse(int warehouseId, [FromQuery] string? term)
        {
            var materials = _materialService.GetByWarehouse(warehouseId, term);
            if (materials == null || !materials.Any())
                return NotFound("No materials found in this warehouse.");

            var result = _mapper.Map<IEnumerable<MaterialDto>>(materials);
            return Ok(result);
        }

    }
}
