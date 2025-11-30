using Application.Common.Pagination;
using Application.Constants.Messages;
using Application.DTOs.Material;
using Application.Interfaces;
using Application.Responses;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;

    public MaterialsController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpPost]
    public IActionResult CreateMaterial([FromBody] CreateMaterialRequest request)
    {
        try
        {
            _materialService.CreateMaterial(request);
            return Ok(ApiResponse<string>.SuccessResponse(MaterialMessages.MSG_MATERIAL_CREATED_SUCCESS));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateMaterial(int id, [FromBody] UpdateMaterialRequest request)
    {
        try
        {
            _materialService.UpdateMaterial(id, request);
            return Ok(ApiResponse<string>.SuccessResponse(MaterialMessages.MSG_MATERIAL_UPDATED_SUCCESS));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteMaterial(int id)
    {
        try
        {
            _materialService.Delete(id);
            return Ok(ApiResponse<string>.SuccessResponse(MaterialMessages.MSG_MATERIAL_DELETED_SUCCESS));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("buyer/{id:int}")]
    public IActionResult GetByIdForBuyer(int id, [FromQuery] int? buyerPartnerId)
    {
        var partnerIdClaim = User.FindFirst("PartnerId")?.Value;

        int? finalBuyerId = partnerIdClaim != null ? int.Parse(partnerIdClaim) : buyerPartnerId;

        if (!finalBuyerId.HasValue)
            return BadRequest(ApiResponse<string>.ErrorResponse("Missing PartnerId (token or query)."));

        var result = _materialService.GetById(id, finalBuyerId);
        if (result == null)
            return NotFound(ApiResponse<string>.ErrorResponse(MaterialMessages.MSG_MATERIAL_NOT_FOUND));

        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetMaterials()
    {
        var materials = _materialService.GetAll();
        return Ok(ApiResponse<IEnumerable<Material>>.SuccessResponse(materials));
    }

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

    [HttpGet("by-category/{categoryId:int}")]
    public IActionResult GetByCategory(int categoryId)
    {
        try
        {
            var materials = _materialService.GetByCategoryOrFail(categoryId);
            return Ok(ApiResponse<IEnumerable<Material>>.SuccessResponse(materials));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("by-warehouse/{warehouseId:int}")]
    public IActionResult GetByWarehouse(int warehouseId, [FromQuery] string? term)
    {
        try
        {
            var materials = _materialService.GetByWarehouseOrFail(warehouseId, term);
            return Ok(ApiResponse<IEnumerable<Material>>.SuccessResponse(materials));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }
}
