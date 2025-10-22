using Application.Common.Pagination;
using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
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

        // POST: api/categories
        [HttpPost]
        public IActionResult Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = new Category
                {
                    CategoryName = request.CategoryName,
                    Description = request.Description
                };

                _categoryService.Create(category);

                return Ok(ApiResponse<string>.SuccessResponse("Category created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }


        // PUT: api/categories/{id}
        [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<Category>> UpdateCategory(int id, [FromBody] CategoryDto dto)
        {
            try
            {
                var existing = _categoryService.GetById(id);
                if (existing == null)
                    return NotFound(ApiResponse<Category>.ErrorResponse("Category not found."));

                var category = _mapper.Map<Category>(dto);
                category.CategoryId = id;

                _categoryService.Update(category);
                return Ok(ApiResponse<Category>.SuccessResponse(category, "Category updated successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Category>.ErrorResponse(ex.Message));
            }
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id:int}")]
        public ActionResult<ApiResponse<string>> DeleteCategory(int id)
        {
            try
            {
                var existing = _categoryService.GetById(id);
                if (existing == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Category not found or already deleted."));

                _categoryService.Delete(id);
                return Ok(ApiResponse<string>.SuccessResponse("Deleted successfully.", "Category and related materials have been soft deleted."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        // GET: api/categories
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = _categoryService.GetAll();
            var result = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(result, "Fetched all categories successfully."));
        }

        // GET: api/categories/{id}
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<CategoryDto>> GetCategory(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null)
                return NotFound(ApiResponse<CategoryDto>.ErrorResponse("Category not found."));

            var result = _mapper.Map<CategoryDto>(category);
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(result, "Fetched category successfully."));
        }

        // GET: api/categories/status/{status}
        [HttpGet("status/{status}")]
        public ActionResult<ApiResponse<IEnumerable<CategoryDto>>> GetByStatus(string status)
        {
            var categories = _categoryService.GetByStatus(status);
            if (categories == null || !categories.Any())
                return NotFound(ApiResponse<IEnumerable<CategoryDto>>.ErrorResponse($"No categories found with status '{status}'."));

            var result = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(result, "Fetched categories by status successfully."));
        }

        // GET: api/categories/filter?SearchTerm=...&PageNumber=1&PageSize=10
        [HttpGet("filter")]
        public ActionResult<ApiResponse<PagedResultDto<CategoryDto>>> GetCategoriesFiltered([FromQuery] PagedQueryDto queryParams)
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

            return Ok(ApiResponse<PagedResultDto<CategoryDto>>.SuccessResponse(result, "Fetched filtered categories successfully."));
        }
    }
}
