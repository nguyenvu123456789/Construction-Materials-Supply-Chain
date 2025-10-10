using Application.Common.Pagination;
using Application.DTOs;
using Application.DTOs.Common.Pagination;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
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

            var latestByMat = checksInRange
                .GroupBy(c => c.MaterialId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CheckDate).First());

            var sysQty = mats.ToDictionary(m => m.MaterialId,
                m => m.Inventories.Sum(i => (decimal?)i.Quantity ?? 0m));

            var avgPrice = mats.ToDictionary(m => m.MaterialId,
                m => m.Inventories.Any() ? (m.Inventories.Average(i => (decimal?)i.UnitPrice) ?? 0m) : 0m);

            int skuWithChecks = latestByMat.Keys.Intersect(mats.Select(m => m.MaterialId)).Count();
            int skuAccurate = 0;
            decimal totalValueDiff = 0m;

            foreach (var m in mats)
            {
                if (!latestByMat.TryGetValue(m.MaterialId, out var last)) continue;
                var actual = (decimal)last.QuantityChecked;
                var system = sysQty[m.MaterialId];
                var diff = actual - system;
                if (diff == 0) skuAccurate++;
                totalValueDiff += diff * avgPrice[m.MaterialId];
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
                query = query.Where(c => matIds.Contains(c.MaterialId));
            }

            var ordered = query.OrderByDescending(c => c.CheckDate).ToList();

            var items = new List<StockCheckListItemDto>();
            foreach (var c in ordered)
            {
                if (!matLookup.TryGetValue(c.MaterialId, out var m)) continue;

                var wh = m.Inventories.FirstOrDefault()?.Warehouse?.WarehouseName ?? "—";
                var sysQty = m.Inventories.Sum(i => (decimal?)i.Quantity ?? 0m);
                var avgPrice = m.Inventories.Any() ? (m.Inventories.Average(i => (decimal?)i.UnitPrice) ?? 0m) : 0m;
                var diffQty = (decimal)c.QuantityChecked - sysQty;
                var diffVal = diffQty * avgPrice;
                var status = (DateTime.UtcNow - c.CheckDate).TotalHours <= 12 ? "Đang" : "Đã duyệt";

                items.Add(new StockCheckListItemDto
                {
                    Code = $"PKK-{c.CheckId:000}",
                    Warehouse = wh,
                    InCharge = $"User #{c.UserId}",
                    Status = status,
                    SkuCount = 1,
                    DiffQty = diffQty,
                    DiffValue = diffVal,
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

            var latestByMat = checks
                .GroupBy(c => c.MaterialId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CheckDate).First());

            var result = new List<SkuDiffDto>();
            foreach (var m in mats)
            {
                var systemQty = (decimal)m.Inventories.Sum(i => (decimal?)i.Quantity ?? 0m);
                var actualQty = latestByMat.TryGetValue(m.MaterialId, out var last)
                    ? (decimal)last.QuantityChecked
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
                    Reason = latestByMat.TryGetValue(m.MaterialId, out var ck) ? (ck.Notes ?? "—") : "—"
                });
            }

            return Paginate(result.OrderBy(r => r.Sku).ToList(), q.PageNumber, q.PageSize);
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
