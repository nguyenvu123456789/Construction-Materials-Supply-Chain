using Application.Common.Pagination;
using Application.DTOs.Material;
using Application.Interfaces;
using Application.Responses;
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

        // POST: api/materials
        [HttpPost]
        public IActionResult CreateMaterial([FromBody] CreateMaterialRequest request)
        {
            try
            {
                var material = new Material
                {
                    MaterialCode = request.MaterialCode,
                    MaterialName = request.MaterialName,
                    CategoryId = request.CategoryId,
                    PartnerId = request.PartnerId,
                    Unit = request.Unit
                };

                _materialService.Create(material);
                return Ok(ApiResponse<string>.SuccessResponse("Material created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        // PUT: api/materials/{id}
        [HttpPut("{id:int}")]
        public IActionResult UpdateMaterial(int id, [FromBody] UpdateMaterialRequest request)
        {
            try
            {
                var existing = _materialService.GetById(id);
                if (existing == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Material not found."));

                var updated = new Material
                {
                    MaterialId = id,
                    MaterialCode = request.MaterialCode,
                    MaterialName = request.MaterialName,
                    CategoryId = request.CategoryId,
                    PartnerId = request.PartnerId,
                    Unit = request.Unit,
                    Status = request.Status
                };

                _materialService.Update(updated);
                return Ok(ApiResponse<string>.SuccessResponse("Material updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        // DELETE: api/materials/{id}
        [HttpDelete("{id:int}")]
        public IActionResult DeleteMaterial(int id)
        {
            try
            {
                var existing = _materialService.GetById(id);
                if (existing == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Material not found."));

                _materialService.Delete(id);
                return Ok(ApiResponse<string>.SuccessResponse("Material soft-deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        // GET: api/materials/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetMaterial(int id)
        {
            var material = _materialService.GetById(id);
            if (material == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Material not found."));

            return Ok(ApiResponse<Material>.SuccessResponse(material));
        }

        // GET: api/materials
        [HttpGet]
        public IActionResult GetMaterials()
        {
            var materials = _materialService.GetAll();
            return Ok(ApiResponse<IEnumerable<Material>>.SuccessResponse(materials));
        }

        // GET: api/materials/filter?SearchTerm=...&PageNumber=1&PageSize=10
        [HttpGet("filter")]
        public IActionResult GetMaterialsFiltered([FromQuery] PagedQueryDto queryParams)
        {
            var materials = _materialService.GetMaterialsFiltered(
                queryParams.SearchTerm,
                queryParams.PageNumber,
                queryParams.PageSize,
                out var totalCount
            );

            var result = new PagedResultDto<Material>
            {
                Data = materials,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize)
            };

            return Ok(ApiResponse<PagedResultDto<Material>>.SuccessResponse(result));
        }

        // GET: api/materials/by-category/{categoryId}
        [HttpGet("by-category/{categoryId:int}")]
        public IActionResult GetByCategory(int categoryId)
        {
            var materials = _materialService.GetByCategory(categoryId);
            if (materials == null || !materials.Any())
                return NotFound(ApiResponse<string>.ErrorResponse("No materials found for this category."));

            return Ok(ApiResponse<IEnumerable<Material>>.SuccessResponse(materials));
        }

        // GET: api/materials/by-warehouse/{warehouseId}
        [HttpGet("by-warehouse/{warehouseId:int}")]
        public IActionResult GetByWarehouse(int warehouseId, [FromQuery] string? term)
        {
            var materials = _materialService.GetByWarehouse(warehouseId, term);
            if (materials == null || !materials.Any())
                return NotFound(ApiResponse<string>.ErrorResponse("No materials found in this warehouse."));

            return Ok(ApiResponse<IEnumerable<Material>>.SuccessResponse(materials));
        }
    }
}
