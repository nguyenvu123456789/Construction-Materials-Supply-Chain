using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class MaterialCheckService : IMaterialCheckService
    {
        private readonly IMaterialRepository _materials;
        private readonly IMaterialCheckRepository _checks;
        private readonly IUserRepository _users;
        private readonly IWarehouseRepository _warehouses;
        private readonly IHandleRequestRepository _handleRequests;

        public MaterialCheckService(
    IMaterialRepository materials,
    IMaterialCheckRepository checks,
    IUserRepository users,
    IWarehouseRepository warehouses,
    IHandleRequestRepository handleRequests)
        {
            _materials = materials;
            _checks = checks;
            _users = users;
            _warehouses = warehouses;
            _handleRequests = handleRequests;
        }

        public ApiResponse<PagedResultDto<MaterialCheckResponseDto>> GetAllMaterialChecks(
    int? partnerId = null,
    int? userId = null,
    string? searchTerm = null,
    int pageNumber = 1,
    int pageSize = 10)
        {
            // Lấy toàn bộ check
            var query = _checks.GetAllWithDetails().AsQueryable();

            // Filter theo partner nếu có
            if (partnerId.HasValue)
                query = query.Where(c => c.User != null && c.User.PartnerId == partnerId.Value);

            // Filter theo user nếu có
            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);

            // Search theo term (username, fullname hoặc warehouse)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    (c.User != null && c.User.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.User != null && c.User.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Warehouse != null && c.Warehouse.WarehouseName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Sắp xếp theo CheckDate giảm dần
            query = query.OrderByDescending(c => c.CheckDate);

            // Paging
            var totalCount = query.Count();
            var paged = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            // Mapping giữ nguyên
            var checks = paged
                .Select(c => new MaterialCheckResponseDto
                {
                    CheckId = c.CheckId,
                    WarehouseId = c.WarehouseId,
                    WarehouseName = c.Warehouse != null ? c.Warehouse.WarehouseName : "—",
                    UserId = c.UserId,
                    UserName = c.User != null ? c.User.UserName : "—",
                    FullName = c.User != null ? c.User.FullName : "—",
                    CheckDate = c.CheckDate,
                    Notes = c.Notes,
                    Status = c.Status
                })
                .ToList();

            var result = new PagedResultDto<MaterialCheckResponseDto>
            {
                Data = checks,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return ApiResponse<PagedResultDto<MaterialCheckResponseDto>>.SuccessResponse(result);
        }



        public ApiResponse<MaterialCheckResponseWithHandleDto> GetMaterialCheckById(int checkId)
        {
            var check = _checks.GetAllWithDetails()
                .FirstOrDefault(c => c.CheckId == checkId);

            if (check == null)
                return ApiResponse<MaterialCheckResponseWithHandleDto>.ErrorResponse($"Phiếu kiểm kho {checkId} không tồn tại.");

            // Lấy handle mới nhất
            var latestHandle = _handleRequests
                .GetAll()
                .Where(h => h.RequestType == "MaterialCheck" && h.RequestId == checkId)
                .OrderByDescending(h => h.HandledAt)
                .FirstOrDefault();

            var response = new MaterialCheckResponseWithHandleDto
            {
                CheckId = check.CheckId,
                WarehouseId = check.WarehouseId,
                WarehouseName = check.Warehouse?.WarehouseName ?? "—",
                UserId = check.UserId,
                UserName = check.User?.UserName ?? "—",
                FullName = check.User?.FullName ?? "—",
                CheckDate = check.CheckDate,
                Notes = check.Notes,
                Status = check.Status,
                Details = check.Details.Select(d => new MaterialCheckDetailResponseDto
                {
                    MaterialId = d.MaterialId,
                    MaterialCode = d.Material?.MaterialCode ?? "",
                    MaterialName = d.Material?.MaterialName ?? "",
                    Unit = d.Material?.Unit,
                    SystemQty = d.SystemQty,
                    ActualQty = d.ActualQty,
                    Reason = d.Reason
                }).ToList(),
                LatestHandle = latestHandle == null ? null : new MaterialCheckHandleResponseDto
                {
                    CheckId = check.CheckId,
                    Status = latestHandle.ActionType switch
                    {
                        "Approve" => "Approved",
                        "Reject" => "Rejected",
                        _ => latestHandle.ActionType
                    },
                    HandledAt = latestHandle.HandledAt,
                    Note = latestHandle.Note
                }
            };

            return ApiResponse<MaterialCheckResponseWithHandleDto>.SuccessResponse(response);
        }



        public ApiResponse<MaterialCheckResponseDto> CreateMaterialCheck(MaterialCheckCreateDto dto)
        {
            if (dto == null || dto.Details == null || !dto.Details.Any())
                return ApiResponse<MaterialCheckResponseDto>.ErrorResponse("Chi tiết kiểm kho không được để trống.");

            var warehouse = _warehouses.GetById(dto.WarehouseId);
            if (warehouse == null)
                return ApiResponse<MaterialCheckResponseDto>.ErrorResponse($"Warehouse {dto.WarehouseId} không tồn tại.");

            var user = _users.GetById(dto.UserId);
            if (user == null)
                return ApiResponse<MaterialCheckResponseDto>.ErrorResponse($"User {dto.UserId} không tồn tại.");

            foreach (var d in dto.Details)
            {
                var material = _materials.GetById(d.MaterialId);
                if (material == null)
                    return ApiResponse<MaterialCheckResponseDto>.ErrorResponse($"Material {d.MaterialId} không tồn tại.");
            }

            var check = new MaterialCheck
            {
                WarehouseId = dto.WarehouseId,
                UserId = dto.UserId,
                CheckDate = dto.CheckDate,
                Notes = dto.Notes,
                Status = dto.Status,
                Details = dto.Details.Select(d => new MaterialCheckDetail
                {
                    MaterialId = d.MaterialId,
                    SystemQty = d.SystemQty,
                    ActualQty = d.ActualQty,
                    Reason = d.Reason
                }).ToList()
            };

            _checks.Add(check); // giả sử Add() đã save changes

            var response = new MaterialCheckResponseDto
            {
                CheckId = check.CheckId,
                WarehouseId = warehouse.WarehouseId,
                WarehouseName = warehouse.WarehouseName,
                UserId = user.UserId,
                UserName = user.UserName,
                FullName = user.FullName,
                CheckDate = check.CheckDate,
                Notes = check.Notes,
                Status = check.Status,
                Details = check.Details.Select(d =>
                {
                    var mat = _materials.GetById(d.MaterialId);
                    return new MaterialCheckDetailResponseDto
                    {
                        MaterialId = d.MaterialId,
                        MaterialCode = mat?.MaterialCode ?? "",
                        MaterialName = mat?.MaterialName ?? "",
                        Unit = mat?.Unit,
                        SystemQty = d.SystemQty,
                        ActualQty = d.ActualQty,
                        Reason = d.Reason
                    };
                }).ToList()
            };

            return ApiResponse<MaterialCheckResponseDto>.SuccessResponse(response, "Tạo phiếu kiểm kho thành công");
        }

        public ApiResponse<MaterialCheckHandleResponseDto> HandleMaterialCheck(MaterialCheckHandleDto dto)
        {
            var check = _checks.GetById(dto.CheckId);
            if (check == null)
                return ApiResponse<MaterialCheckHandleResponseDto>.ErrorResponse($"Phiếu kiểm kho {dto.CheckId} không tồn tại.");

            var user = _users.GetById(dto.HandledBy);
            if (user == null)
                return ApiResponse<MaterialCheckHandleResponseDto>.ErrorResponse($"User {dto.HandledBy} không tồn tại.");

            // Xác định trạng thái mới
            if (dto.Action != "Approved" && dto.Action != "Rejected")
                return ApiResponse<MaterialCheckHandleResponseDto>.ErrorResponse("Action phải là 'Approved' hoặc 'Rejected'.");

            check.Status = dto.Action == "Approve" ? "Approved" : "Rejected";

            // Lưu hành động vào HandleRequest
            var handle = new HandleRequest
            {
                RequestType = "MaterialCheck",
                RequestId = check.CheckId,
                HandledBy = dto.HandledBy,
                ActionType = check.Status,
                Note = dto.Note,
                HandledAt = DateTime.UtcNow
            };
            _handleRequests.Add(handle); // giả sử Add() lưu changes

            return ApiResponse<MaterialCheckHandleResponseDto>.SuccessResponse(new MaterialCheckHandleResponseDto
            {
                CheckId = check.CheckId,
                Status = check.Status,
                HandledAt = handle.HandledAt,
                Note = handle.Note
            }, $"Phiếu kiểm kho đã được {check.Status.ToLower()}");
        }

    }
}
