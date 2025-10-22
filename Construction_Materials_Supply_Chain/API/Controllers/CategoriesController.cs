using Application.Common.Pagination;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }


        [HttpPost]
        public IActionResult CreateCategory(CategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            _categoryService.Create(category);
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, _mapper.Map<CategoryDto>(category));
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateCategory(int id, CategoryDto dto)
        {
            var existing = _categoryService.GetById(id);
            if (existing == null) return NotFound();

            var category = _mapper.Map<Category>(dto);
            category.CategoryId = id;
            _categoryService.Update(category);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteCategory(int id)
        {
            var existing = _categoryService.GetById(id);
            if (existing == null)
                return NotFound("Category not found or already deleted.");

            _categoryService.Delete(id);
            return Ok("Category and related materials have been soft deleted.");
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoryDto>> GetCategories()
        {
            var categories = _categoryService.GetAll();
            var result = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<CategoryDto> GetCategory(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null) return NotFound();
            return Ok(_mapper.Map<CategoryDto>(category));
        }
        // GET: api/categories/status/{status}
        [HttpGet("status/{status}")]
        public ActionResult<IEnumerable<CategoryDto>> GetByStatus(string status)
        {
            var categories = _categoryService.GetByStatus(status);
            if (categories == null || !categories.Any())
                return NotFound($"No categories found with status '{status}'.");

            var result = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(result);
        }


        // GET: api/categories/filter?SearchTerm=...&PageNumber=1&PageSize=10
        [HttpGet("filter")]
        public ActionResult<PagedResultDto<CategoryDto>> GetCategoriesFiltered([FromQuery] PagedQueryDto queryParams)
        {
            var categories = _categoryService.GetCategoriesFiltered(
                queryParams.SearchTerm,
                queryParams.PageNumber,
                queryParams.PageSize,
                out var totalCount
            );

            var result = new PagedResultDto<CategoryDto>
            {
                Data = _mapper.Map<IEnumerable<CategoryDto>>(categories),
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize)
            };

            return Ok(result);
        }
    }
}
