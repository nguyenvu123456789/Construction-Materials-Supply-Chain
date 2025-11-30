using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Common.Pagination;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class StockCheckService : IStockCheckService
    {
        private readonly IMaterialRepository _materials;
        private readonly IMaterialCheckRepository _checks;

        public StockCheckService(IMaterialRepository materials, IMaterialCheckRepository checks)
        {
            _materials = materials;
            _checks = checks;
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

            var checksInRange = _checks.GetAll()
                .Where(c => (!q.From.HasValue || c.CheckDate >= q.From.Value)
                         && (!q.To.HasValue || c.CheckDate <= q.To.Value))
                .ToList();

            var latestDetail = checksInRange
                .SelectMany(c => c.Details)
                .GroupBy(d => d.MaterialId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Check.CheckDate).First());

            int skuWithChecks = latestDetail.Keys.Count;
            int skuAccurate = 0;

            foreach (var m in mats)
            {
                if (!latestDetail.TryGetValue(m.MaterialId, out var last)) continue;

                var diff = last.ActualQty - last.SystemQty;
                if (diff == 0) skuAccurate++;
            }

            var accuracy = skuWithChecks == 0 ? 0 : (double)skuAccurate / skuWithChecks * 100;
            var today = DateTime.Now.Date;

            int activeChecks = checksInRange.Count(c => c.CheckDate >= today);

            return new StockCheckSummaryDto
            {
                TotalSkuNeedCheck = mats.Count,
                AccuracyPercent = Math.Round(accuracy, 1),
                TotalValueDiff = 0,
                ActiveCheckCount = activeChecks
            };
        }

        public PagedResultDto<StockCheckListItemDto> GetChecks(StockCheckQueryDto q)
        {
            var checks = _checks.GetAll()
                .Where(c => (!q.From.HasValue || c.CheckDate >= q.From.Value)
                         && (!q.To.HasValue || c.CheckDate <= q.To.Value))
                .OrderByDescending(c => c.CheckDate)
                .ToList();

            var items = new List<StockCheckListItemDto>();

            foreach (var c in checks)
            {
                // tổng số sku trong phiếu
                int skuCount = c.Details.Count;

                decimal totalDiff = c.Details.Sum(d => d.ActualQty - d.SystemQty);

                var firstMat = c.Details.FirstOrDefault();
                string warehouseName = firstMat?.Material?.Inventories.FirstOrDefault()?.Warehouse?.WarehouseName ?? "—";

                var status = (DateTime.Now - c.CheckDate).TotalHours <= 12 ? "Đang" : "Đã duyệt";

                items.Add(new StockCheckListItemDto
                {
                    Code = $"PKK-{c.CheckId:000}",
                    Warehouse = warehouseName,
                    InCharge = $"User #{c.UserId}",
                    Status = status,
                    SkuCount = skuCount,
                    DiffQty = totalDiff,
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

            var latestDetail = _checks.GetAll()
                .Where(c => (!q.From.HasValue || c.CheckDate >= q.From.Value)
                         && (!q.To.HasValue || c.CheckDate <= q.To.Value))
                .SelectMany(c => c.Details)
                .GroupBy(d => d.MaterialId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Check.CheckDate).First());

            var result = new List<SkuDiffDto>();

            foreach (var m in mats)
            {
                var systemQty = m.Inventories.Sum(i => i.Quantity ?? 0m);

                var actualQty = latestDetail.TryGetValue(m.MaterialId, out var last)
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
                    Reason = latestDetail.TryGetValue(m.MaterialId, out var lastDetail)
                                ? lastDetail.Reason ?? "—"
                                : "—"
                });
            }

            return Paginate(result, q.PageNumber, q.PageSize);
        }

        private List<Material> GetMaterialsScoped(StockCheckQueryDto q)
        {
            return q.WarehouseId.HasValue
                ? _materials.GetByWarehouse(q.WarehouseId.Value)
                : _materials.GetAllWithInventory();
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
    }
}
