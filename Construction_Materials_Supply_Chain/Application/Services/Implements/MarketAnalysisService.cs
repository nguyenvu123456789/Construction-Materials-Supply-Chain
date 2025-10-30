using Application.DTOs;
using Application.DTOs.Application.DTOs;
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
        private readonly IInvoiceRepository _invoices;

        public MarketAnalysisService(
            IOrderRepository orders,
            IOrderDetailRepository orderDetails,
            IPartnerRepository partners,
            IUserRepository users,
            IMaterialRepository materials,
            IInvoiceRepository invoices)
        {
            _orders = orders;
            _orderDetails = orderDetails;
            _partners = partners;
            _users = users;
            _materials = materials;
            _invoices = invoices;
        }

        // 🔹 1. Top vật tư bán chạy
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

        // 🔹 2. Doanh thu theo nhà cung cấp
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

        public List<WeeklyRevenueDto> GetWeeklyRevenueByPartner(int userId)
        {
            var invoices = _invoices.GetAll()
                .Where(i => i.Status == "Success" && i.CreatedBy == userId)
                .ToList();

            if (!invoices.Any())
                return new List<WeeklyRevenueDto>();

            var culture = CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;

            var grouped = invoices
                .GroupBy(i =>
                {
                    int weekNum = calendar.GetWeekOfYear(i.IssueDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    int year = i.IssueDate.Year;

                    DateTime weekStart = FirstDateOfWeek(year, weekNum);
                    DateTime weekEnd = weekStart.AddDays(6);

                    return new
                    {
                        i.PartnerId,
                        PartnerName = i.Partner?.PartnerName ?? "Không xác định",
                        WeekStart = weekStart,
                        WeekEnd = weekEnd
                    };
                })
                .Select(g => new WeeklyRevenueDto
                {
                    PartnerId = g.Key.PartnerId,
                    PartnerName = g.Key.PartnerName,
                    WeekStart = g.Key.WeekStart,
                    WeekEnd = g.Key.WeekEnd,
                    TotalRevenue = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.WeekStart)
                .ToList();

            return grouped;
        }
        private DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;

            var firstMonday = jan1.AddDays(daysOffset);
            var firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            if (firstWeek <= 1)
            {
                weekOfYear -= 1;
            }

            return firstMonday.AddDays(weekOfYear * 7);
        }

    }
}
