using Application.Common.Pagination;
using Application.Constants.Messages;
using Application.Interfaces;
using Application.Responses;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] Category category)
        {
            try
            {
                _categoryService.Create(category);
                return Ok(ApiResponse<string>.SuccessResponse(CategoryMessages.MSG_CATEGORY_CREATED_SUCCESS));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Category category)
        {
            try
            {
                category.CategoryId = id;
                _categoryService.Update(category);
                return Ok(ApiResponse<string>.SuccessResponse(CategoryMessages.MSG_CATEGORY_UPDATED_SUCCESS));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _categoryService.Delete(id);
                return Ok(ApiResponse<string>.SuccessResponse(CategoryMessages.MSG_CATEGORY_DELETED_SUCCESS));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null)
                return NotFound(ApiResponse<string>.ErrorResponse(CategoryMessages.MSG_CATEGORY_NOT_FOUND));

            return Ok(category);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _categoryService.GetAll();
            return Ok(categories);
        }

        [HttpGet("status/{status}")]
        public IActionResult GetByStatus(string status)
        {
            var categories = _categoryService.GetByStatus(status);
            return Ok(categories);
        }

        [HttpGet("filter")]
        public IActionResult GetFiltered([FromQuery] PagedQueryDto queryParams)
        {
            var categories = _categoryService.GetCategoriesFiltered(
                queryParams.SearchTerm,
                queryParams.PageNumber,
                queryParams.PageSize,
                out var totalCount
            );

            var result = new PagedResultDto<Category>
            {
                Data = categories,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize)
            };

            return Ok(result);
        }
    }
}
