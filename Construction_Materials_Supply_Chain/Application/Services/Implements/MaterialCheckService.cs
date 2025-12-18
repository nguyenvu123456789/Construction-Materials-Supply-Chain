using Application.Common.Pagination;
using Application.Constants.Enums;
using Application.Constants.Messages;
using Application.DTOs;
using Application.DTOs.Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Domain.Interface;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implements
{
    public class MaterialCheckService : IMaterialCheckService
    {
        private readonly IMaterialRepository _materials;
        private readonly IMaterialCheckRepository _checks;
        private readonly IUserRepository _users;
        private readonly IWarehouseRepository _warehouses;
        private readonly IHandleRequestRepository _handleRequests;
        private readonly IInventoryRepository _inventories;

        public MaterialCheckService(
            IMaterialRepository materials,
            IMaterialCheckRepository checks,
            IUserRepository users,
            IWarehouseRepository warehouses,
            IHandleRequestRepository handleRequests,
            IInventoryRepository inventories)
        {
            _materials = materials;
            _checks = checks;
            _users = users;
            _warehouses = warehouses;
            _handleRequests = handleRequests;
            _inventories = inventories;
        }

        public ApiResponse<PagedResultDto<MaterialCheckResponseDto>> GetAllMaterialChecks(
            int? partnerId = null,
            int? userId = null,
            string? searchTerm = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            // Base query
            var query = _checks.GetAllWithDetails().AsQueryable();
            // Filter: partner
            if (partnerId.HasValue)
            {
                query = query.Where(c => c.User != null && c.User.PartnerId == partnerId.Value);
            }
            // Filter: user
            if (userId.HasValue)
            {
                query = query.Where(c => c.UserId == userId.Value);
            }
            // Filter: search text
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var keyword = $"%{searchTerm}%";

                query = query.Where(c =>
                    (c.User != null && EF.Functions.Like(c.User.UserName, keyword)) ||
                    (c.User != null && EF.Functions.Like(c.User.FullName, keyword)) ||
                    (c.Warehouse != null && EF.Functions.Like(c.Warehouse.WarehouseName, keyword))
                );
            }
            // Filter: status
            if (!string.IsNullOrWhiteSpace(status))
            {
                var st = status.Trim().ToLower();
                query = query.Where(c => c.Status.ToLower() == st);
            }
            // Filter: CheckDate from
            if (fromDate.HasValue)
            {
                query = query.Where(c => c.CheckDate >= fromDate.Value);
            }
            // Filter: CheckDate to
            if (toDate.HasValue)
            {
                // Nếu bạn muốn bao trọn ngày cuối
                var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.CheckDate <= endOfDay);
            }
            // Sort: newest first
            query = query.OrderByDescending(c => c.CheckDate);
            // Pagination
            var totalCount = query.Count();
            var items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();
            // Mapping
            var data = items.Select(c => new MaterialCheckResponseDto
            {
                CheckId = c.CheckId,
                WarehouseId = c.WarehouseId,
                WarehouseName = c.Warehouse?.WarehouseName ?? "—",
                UserId = c.UserId,
                UserName = c.User?.UserName ?? "—",
                FullName = c.User?.FullName ?? "—",
                CheckDate = c.CheckDate,
                Notes = c.Notes,
                Status = c.Status
            }).ToList();

            var result = new PagedResultDto<MaterialCheckResponseDto>
            {
                Data = data,
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
                return ApiResponse<MaterialCheckResponseWithHandleDto>.ErrorResponse(MaterialCheckMessages.INVALID_CHECK_DETAILS);

            // Lấy handle mới nhất
            var latestHandle = _handleRequests
                .GetAll()
                .Where(h => h.RequestType == StatusEnum.MaterialCheck.ToStatusString() && h.RequestId == checkId)
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
            // Validate request
            if (dto == null || dto.Details == null || !dto.Details.Any())
                return ApiResponse<MaterialCheckResponseDto>
                    .ErrorResponse(MaterialCheckMessages.INVALID_CHECK_DETAILS);

            // Validate warehouse
            var warehouse = _warehouses.GetById(dto.WarehouseId);
            if (warehouse == null)
                return ApiResponse<MaterialCheckResponseDto>
                    .ErrorResponse(string.Format(
                        MaterialCheckMessages.WAREHOUSE_NOT_FOUND, dto.WarehouseId));

            //  Validate user
            var user = _users.GetById(dto.UserId);
            if (user == null)
                return ApiResponse<MaterialCheckResponseDto>
                    .ErrorResponse(string.Format(
                        MaterialCheckMessages.USER_NOT_FOUND, dto.UserId));

            //  Tạo phiếu kiểm kho
            var check = new MaterialCheck
            {
                WarehouseId = dto.WarehouseId,
                UserId = dto.UserId,
                CheckDate = DateTime.Now,
                Notes = dto.Notes,
                Status = StatusEnum.Pending.ToStatusString(),
                Details = new List<MaterialCheckDetail>()
            };

            // Tạo chi tiết kiểm kho
            foreach (var d in dto.Details)
            {
                var material = _materials.GetById(d.MaterialId);
                if (material == null)
                    return ApiResponse<MaterialCheckResponseDto>
                        .ErrorResponse(string.Format(
                            MaterialCheckMessages.MATERIAL_NOT_FOUND, d.MaterialId));

                var inventory = _inventories
                    .GetByWarehouseAndMaterial(dto.WarehouseId, d.MaterialId);

                check.Details.Add(new MaterialCheckDetail
                {
                    MaterialId = d.MaterialId,
                    SystemQty = inventory?.Quantity ?? 0,  
                    ActualQty = d.ActualQty,
                    Reason = d.Reason
                });
            }

            _checks.Add(check);

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

            return ApiResponse<MaterialCheckResponseDto>
                .SuccessResponse(response, MaterialCheckMessages.CREATE_SUCCESS);
        }

        public ApiResponse<MaterialCheckHandleResponseDto> HandleMaterialCheck(MaterialCheckHandleDto dto)
        {
            var check = _checks.GetByIdWithDetails(dto.CheckId);
            if (check == null)
                return ApiResponse<MaterialCheckHandleResponseDto>
                    .ErrorResponse(string.Format(
                        MaterialCheckMessages.CHECK_NOT_FOUND, dto.CheckId));

            var user = _users.GetById(dto.HandledBy);
            if (user == null)
                return ApiResponse<MaterialCheckHandleResponseDto>
                    .ErrorResponse(string.Format(
                        MaterialCheckMessages.USER_NOT_FOUND, dto.HandledBy));

            if (dto.Action != StatusEnum.Approved.ToStatusString() &&
                dto.Action != StatusEnum.Rejected.ToStatusString())
                return ApiResponse<MaterialCheckHandleResponseDto>
                    .ErrorResponse(MaterialCheckMessages.INVALID_ACTION);

            check.Status = dto.Action;
            _checks.Update(check);

            //  Nếu approve thì cập nhật tồn kho
            if (dto.Action == StatusEnum.Approved.ToStatusString())
            {
                ApplyInventoryAfterApproved(check);
            }

            // Lưu lịch sử xử lý
            var handle = new HandleRequest
            {
                RequestType = StatusEnum.MaterialCheck.ToStatusString(),
                RequestId = check.CheckId,
                HandledBy = dto.HandledBy,
                ActionType = check.Status,
                Note = dto.Note,
                HandledAt = DateTime.Now
            };

            _handleRequests.Add(handle);

            return ApiResponse<MaterialCheckHandleResponseDto>
                .SuccessResponse(
                    new MaterialCheckHandleResponseDto
                    {
                        CheckId = check.CheckId,
                        Status = check.Status,
                        HandledAt = handle.HandledAt,
                        Note = handle.Note
                    },
                    string.Format(
                        MaterialCheckMessages.HANDLE_SUCCESS,
                        check.Status.ToLower()
                    )
                );
        }

        private void ApplyInventoryAfterApproved(MaterialCheck check)
        {
            foreach (var detail in check.Details)
            {
                var inventory = _inventories.GetByWarehouseAndMaterial(
                    check.WarehouseId,
                    detail.MaterialId
                );

                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        WarehouseId = check.WarehouseId,
                        MaterialId = detail.MaterialId,
                        Quantity = detail.ActualQty,
                        CreatedAt = DateTime.Now
                    };

                    _inventories.Add(inventory);
                }
                else
                {
                    inventory.Quantity = detail.ActualQty;
                    inventory.UpdatedAt = DateTime.Now;

                    _inventories.Update(inventory);
                }
            }
        }

    }
}
