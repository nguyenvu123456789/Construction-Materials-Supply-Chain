using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using System.Globalization;

namespace Application.Services.Implements
{
    public class MarketAnalysisService : IMarketAnalysisService
    {
        private readonly IOrderRepository _orders;
        private readonly IOrderDetailRepository _orderDetails;
        private readonly IPartnerRepository _partners;
        private readonly IUserRepository _users;
        private readonly IMaterialRepository _materials;

        public MarketAnalysisService(
            IOrderRepository orders,
            IOrderDetailRepository orderDetails,
            IPartnerRepository partners,
            IUserRepository users,
            IMaterialRepository materials)
        {
            _orders = orders;
            _orderDetails = orderDetails;
            _partners = partners;
            _users = users;
            _materials = materials;
        }

        public List<MonthlyRevenueDto> GetMonthlyRevenue()
        {
            var orders = _orders.GetAll()
                .Where(o => o.Status == "Success" && o.CreatedAt != null)
                .ToList();

            var data = orders
                .SelectMany(o => o.OrderDetails, (o, d) => new
                {
                    Year = o.CreatedAt!.Value.Year,
                    Month = o.CreatedAt!.Value.Month,
                    Revenue = (d.UnitPrice ?? 0) * d.Quantity
                })
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new MonthlyRevenueDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList();

            return data;
        }

        public List<TopMaterialDto> GetTopMaterials(int top = 5)
        {
            var details = _orderDetails.GetAll()
                .Where(d => d.Order.Status == "Success")
                .GroupBy(d => new { d.MaterialId, d.Material.MaterialName })
                .Select(g => new TopMaterialDto
                {
                    MaterialId = g.Key.MaterialId,
                    MaterialName = g.Key.MaterialName,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => (x.UnitPrice ?? 0) * x.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(top)
                .ToList();

            return details;
        }

        public List<SupplierRevenueDto> GetRevenueBySupplier()
        {
            var orders = _orders.GetAll()
                .Where(o => o.Status == "Success" && o.SupplierId != null)
                .ToList();

            var data = orders
                .SelectMany(o => o.OrderDetails, (o, d) => new
                {
                    SupplierId = o.SupplierId!.Value,
                    SupplierName = o.Supplier!.PartnerName,
                    Revenue = (d.UnitPrice ?? 0) * d.Quantity
                })
                .GroupBy(x => new { x.SupplierId, x.SupplierName })
                .Select(g => new SupplierRevenueDto
                {
                    SupplierId = g.Key.SupplierId,
                    SupplierName = g.Key.SupplierName,
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            return data;
        }

        public List<StaffPerformanceDto> GetRevenueByStaff()
        {
            var orders = _orders.GetAll()
                .Where(o => o.Status == "Success" && o.CreatedBy != null)
                .ToList();

            var data = orders
                .SelectMany(o => o.OrderDetails, (o, d) => new
                {
                    UserId = o.CreatedBy!.Value,
                    Fullname = o.CreatedByNavigation!.FullName,
                    Revenue = (d.UnitPrice ?? 0) * d.Quantity
                })
                .GroupBy(x => new { x.UserId, x.Fullname })
                .Select(g => new StaffPerformanceDto
                {
                    UserId = g.Key.UserId,
                    Fullname = g.Key.Fullname,
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            return data;
        }

        public List<RegionRevenueDto> GetRevenueByRegion()
        {
            var orders = _orders.GetAll()
                .Where(o => o.Status == "Success" && o.DeliveryAddress != null)
                .ToList();

            var data = orders
                .SelectMany(o => o.OrderDetails, (o, d) => new
                {
                    Region = o.DeliveryAddress!,
                    Revenue = (d.UnitPrice ?? 0) * d.Quantity
                })
                .GroupBy(x => x.Region)
                .Select(g => new RegionRevenueDto
                {
                    Region = g.Key,
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            return data;
        }
    }
}
