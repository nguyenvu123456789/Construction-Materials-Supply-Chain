using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Application.DTOs;
using Application.DTOs.Common.Pagination;
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

        public ApiResponse<List<MaterialCheckResponseDto>> GetAllMaterialChecks(int? partnerId = null)
        {
            // Load toàn bộ check vào memory
            var allChecks = _checks.GetAllWithDetails().ToList();

            // Lọc theo partnerId nếu được truyền vào
            if (partnerId.HasValue)
            {
                allChecks = allChecks
                    .Where(c => c.User != null && c.User.PartnerId == partnerId.Value)
                    .ToList();
            }

            var checks = allChecks
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

            return ApiResponse<List<MaterialCheckResponseDto>>.SuccessResponse(checks);
        }


        public ApiResponse<MaterialCheckResponseWithHandleDto> GetMaterialCheckById(int checkId)
        {
            // Lấy phiếu kiểm kho kèm chi tiết và navigation
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


        public StockCheckSummaryDto GetSummary(StockCheckQueryDto q)
        {
            var mats = GetMaterialsScoped(q);
            if (!string.IsNullOrWhiteSpace(q.SearchTerm))
            {
                mats = mats.Where(m =>
                        (m.MaterialName ?? "").Contains(q.SearchTerm!, StringComparison.OrdinalIgnoreCase) ||
                        (m.MaterialCode ?? "").Contains(q.SearchTerm!, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Lấy tất cả MaterialCheck trong khoảng thời gian
            var checksInRange = _checks.GetAllWithDetails()
                .Where(c => (!q.From.HasValue || c.CheckDate >= q.From.Value)
                    && (!q.To.HasValue || c.CheckDate <= q.To.Value))
                .ToList();


            // Flatten details và lấy bản ghi mới nhất cho mỗi material
            var detailEntries = checksInRange
                .SelectMany(c => c.Details.Select(d => new
                {
                    CheckDate = c.CheckDate,
                    CheckId = c.CheckId,
                    WarehouseId = c.WarehouseId,
                    UserId = c.UserId,
                    MaterialId = d.MaterialId,
                    ActualQty = d.ActualQty,
                    SystemQty = d.SystemQty,
                    Reason = d.Reason,
                    CheckNotes = c.Notes
                }))
                .ToList();

            var latestByMat = detailEntries
                .GroupBy(x => x.MaterialId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CheckDate).First());

            var sysQty = mats.ToDictionary(m => m.MaterialId,
                m => m.Inventories.Sum(i => i.Quantity ?? 0m));

            int skuWithChecks = latestByMat.Keys.Intersect(mats.Select(m => m.MaterialId)).Count();
            int skuAccurate = 0;
            decimal totalValueDiff = 0m;

            foreach (var m in mats)
            {
                if (!latestByMat.TryGetValue(m.MaterialId, out var last)) continue;
                var actual = (decimal)last.ActualQty;
                var system = sysQty[m.MaterialId];
                var diff = actual - system;
                if (diff == 0) skuAccurate++;

                // nếu muốn tính tổng giá trị chênh lệch, cần biết đơn giá; tạm cộng theo qty.
                totalValueDiff += Math.Abs(diff); // placeholder: tổng chênh lệch số lượng
            }

            var accuracy = skuWithChecks == 0 ? 0 : (double)skuAccurate / skuWithChecks * 100.0;
            var today = DateTime.UtcNow.Date;
            int activeChecks = checksInRange.Count(c => c.CheckDate >= today);

            return new StockCheckSummaryDto
            {
                TotalSkuNeedCheck = mats.Count,
                AccuracyPercent = Math.Round(accuracy, 1),
                TotalValueDiff = totalValueDiff,
                ActiveCheckCount = activeChecks
            };
        }

        public PagedResultDto<StockCheckListItemDto> GetChecks(StockCheckQueryDto q)
        {
            var mats = GetMaterialsScoped(q);
            var matLookup = mats.ToDictionary(m => m.MaterialId, m => m);

            // lấy các check trong khoảng
            var query = _checks.GetAll()
                .Where(c => (!q.From.HasValue || c.CheckDate >= q.From.Value)
                         && (!q.To.HasValue || c.CheckDate <= q.To.Value));

            if (!string.IsNullOrWhiteSpace(q.SearchTerm))
            {
                var matIds = mats
                    .Where(m => (m.MaterialName ?? "").Contains(q.SearchTerm!, StringComparison.OrdinalIgnoreCase)
                             || (m.MaterialCode ?? "").Contains(q.SearchTerm!, StringComparison.OrdinalIgnoreCase))
                    .Select(m => m.MaterialId)
                    .ToHashSet();

                // giữ các check có ít nhất một detail thuộc matIds
                query = query.Where(c => c.Details.Any(d => matIds.Contains(d.MaterialId)));
            }

            var ordered = query.OrderByDescending(c => c.CheckDate).ToList();

            var items = new List<StockCheckListItemDto>();
            foreach (var c in ordered)
            {
                // lấy các material trong phiếu thuộc phạm vi mats (nếu user filter theo warehouse/partner)
                var detailsInScope = c.Details.Where(d => matLookup.ContainsKey(d.MaterialId)).ToList();
                if (!detailsInScope.Any()) continue;

                var wh = c.Warehouse?.WarehouseName ?? detailsInScope
                    .Select(d => matLookup.TryGetValue(d.MaterialId, out var mm) ? mm.Inventories.FirstOrDefault()?.Warehouse?.WarehouseName : null)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "—";

                // tính tổng chênh lệch của phiếu
                decimal totalSys = 0m;
                decimal totalActual = 0m;
                foreach (var d in detailsInScope)
                {
                    var system = matLookup.TryGetValue(d.MaterialId, out var mm) ? mm.Inventories.Sum(i => i.Quantity ?? 0m) : 0m;
                    totalSys += system;
                    totalActual += d.ActualQty;
                }

                var diffQty = totalActual - totalSys;
                var status = (DateTime.UtcNow - c.CheckDate).TotalHours <= 12 ? "Đang" : "Đã duyệt";

                items.Add(new StockCheckListItemDto
                {
                    Code = $"PKK-{c.CheckId:000}",
                    Warehouse = wh,
                    InCharge = $"User #{c.UserId}",
                    Status = status,
                    SkuCount = detailsInScope.Count,
                    DiffQty = diffQty,
                    CheckedAt = c.CheckDate
                });
            }

            return Paginate(items, q.PageNumber, q.PageSize);
        }

        public PagedResultDto<SkuDiffDto> GetSkuDiffs(StockCheckQueryDto q)
        {
            var mats = GetMaterialsScoped(q);

            if (!string.IsNullOrWhiteSpace(q.SearchTerm))
            {
                mats = mats.Where(m =>
                        (m.MaterialName ?? "").Contains(q.SearchTerm!, StringComparison.OrdinalIgnoreCase) ||
                        (m.MaterialCode ?? "").Contains(q.SearchTerm!, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var checks = _checks.GetAll()
                .Where(c => (!q.From.HasValue || c.CheckDate >= q.From.Value)
                         && (!q.To.HasValue || c.CheckDate <= q.To.Value))
                .ToList();

            var detailEntries = checks
                .SelectMany(c => c.Details.Select(d => new
                {
                    CheckDate = c.CheckDate,
                    CheckId = c.CheckId,
                    WarehouseId = c.WarehouseId,
                    UserId = c.UserId,
                    MaterialId = d.MaterialId,
                    ActualQty = d.ActualQty,
                    SystemQty = d.SystemQty,
                    Reason = d.Reason,
                    CheckNotes = c.Notes
                }))
                .ToList();

            var latestByMat = detailEntries
                .GroupBy(x => x.MaterialId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CheckDate).First());

            var result = new List<SkuDiffDto>();
            foreach (var m in mats)
            {
                var systemQty = m.Inventories.Sum(i => i.Quantity ?? 0m);
                var actualQty = latestByMat.TryGetValue(m.MaterialId, out var last)
                    ? last.ActualQty
                    : systemQty;

                var diff = actualQty - systemQty;
                if (diff == 0) continue;

                result.Add(new SkuDiffDto
                {
                    Sku = m.MaterialCode ?? $"VT-{m.MaterialId:000}",
                    MaterialName = m.MaterialName,
                    Location = m.Inventories.FirstOrDefault()?.Warehouse?.WarehouseName ?? "—",
                    SystemQty = systemQty,
                    ActualQty = actualQty,
                    DiffQty = diff,
                    Reason = latestByMat.TryGetValue(m.MaterialId, out var ck) ? ck.Reason ?? ck.CheckNotes ?? "—" : "—"
                });
            }

            return Paginate(result.OrderBy(r => r.Sku).ToList(), q.PageNumber, q.PageSize);
        }

        private List<Material> GetMaterialsScoped(StockCheckQueryDto q)
        {
            var mats = q.WarehouseId.HasValue
                ? _materials.GetByWarehouse(q.WarehouseId.Value)
                : _materials.GetAllWithInventory();

            if (q.PartnerId.HasValue)
                mats = mats.Where(m => m.MaterialPartners.Any(mp => mp.PartnerId == q.PartnerId.Value)).ToList();

            return mats;
        }

        private static PagedResultDto<T> Paginate<T>(IEnumerable<T> src, int page, int size)
        {
            page = page <= 0 ? 1 : page;
            size = size <= 0 ? 10 : size;
            var total = src.Count();
            var totalPages = (int)Math.Ceiling(total / (double)size);
            var data = src.Skip((page - 1) * size).Take(size).ToList();

            return new PagedResultDto<T>
            {
                Data = data,
                TotalCount = total,
                PageNumber = page,
                PageSize = size,
                TotalPages = totalPages
            };
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
