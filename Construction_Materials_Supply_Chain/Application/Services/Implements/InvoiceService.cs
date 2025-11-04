using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoices;
        private readonly IMaterialRepository _materials;
        private readonly IInventoryRepository _inventories;
        private readonly IImportRepository _imports;
        private readonly IOrderRepository _orderRepository;

        public InvoiceService(
            IInvoiceRepository invoices,
            IMaterialRepository materials,
            IInventoryRepository inventories,
            IImportRepository imports,
            IOrderRepository orderRepository)
        {
            _invoices = invoices;
            _materials = materials;
            _inventories = inventories;
            _imports = imports;
            _orderRepository = orderRepository;
        }

        public Invoice CreateInvoice(CreateInvoiceDto dto)
        {
            if (_invoices.GetByCode(dto.InvoiceCode) != null)
                throw new Exception("InvoiceCode already exists.");

            var invoice = new Invoice
            {
                InvoiceCode = dto.InvoiceCode,
                InvoiceType = dto.InvoiceType,
                PartnerId = dto.PartnerId,
                CreatedBy = dto.CreatedBy,
                IssueDate = dto.IssueDate,
                DueDate = dto.DueDate,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in dto.Details)
            {
                var material = _materials.GetById(item.MaterialId);
                if (material == null) throw new Exception($"MaterialId {item.MaterialId} not found.");

                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.Quantity * item.UnitPrice
                });
            }

            invoice.TotalAmount = invoice.InvoiceDetails.Sum(d => d.LineTotal ?? 0);

            _invoices.Add(invoice);

            return invoice;
        }

        public Invoice? GetByIdWithDetails(int id) => _invoices.GetByIdWithDetails(id);

        public List<Invoice> GetAllWithDetails() => _invoices.GetAllWithDetails();

        public Invoice? RejectInvoice(int id)
        {
            var invoice = _invoices.GetByIdWithDetails(id);
            if (invoice == null)
                return null;
            invoice.Status = "Rejected";
            invoice.UpdatedAt = DateTime.UtcNow;
            _invoices.Update(invoice);

            return invoice;
        }

        public List<Invoice> CreateInvoiceFromOrder(CreateInvoiceFromOrderDto dto)
        {
            var order = _orderRepository.GetByCode(dto.OrderCode);
            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status != "Approved")
                throw new Exception("Order must be approved to create invoices.");

            var partnerId = order.CreatedByNavigation?.PartnerId;
            if (partnerId == null || partnerId == 0)
                throw new Exception("PartnerId (supplier) not found for this order.");

            if (dto.UnitPrices == null || !dto.UnitPrices.Any())
                throw new Exception("At least one material must be provided for invoicing.");

            // ✅ Lấy danh sách vật tư được chọn từ DTO
            var selectedMaterialIds = dto.UnitPrices.Select(u => u.MaterialId).ToList();

            // ✅ Lọc những vật tư trong order có mặt trong danh sách được chọn
            var selectedDetails = order.OrderDetails
                .Where(od => selectedMaterialIds.Contains(od.MaterialId))
                .ToList();

            if (!selectedDetails.Any())
                throw new Exception("No matching materials found in the order.");

            var createdInvoices = new List<Invoice>();

            foreach (var od in selectedDetails)
            {
                // Lấy giá theo MaterialId
                var unitPriceDto = dto.UnitPrices.FirstOrDefault(u => u.MaterialId == od.MaterialId);
                if (unitPriceDto == null)
                    continue; // bỏ qua nếu không có giá

                var unitPrice = unitPriceDto.UnitPrice;
                var lineTotal = od.Quantity * unitPrice;

                var invoice = new Invoice
                {
                    InvoiceCode = $"INV-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
                    InvoiceType = "Export",
                    PartnerId = partnerId.Value,
                    CreatedBy = dto.CreatedBy,
                    IssueDate = dto.IssueDate,
                    DueDate = dto.DueDate,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    TotalAmount = lineTotal
                };

                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    MaterialId = od.MaterialId,
                    Quantity = od.Quantity,
                    UnitPrice = unitPrice,
                    LineTotal = lineTotal
                });

                _invoices.Add(invoice);
                createdInvoices.Add(invoice);

                od.Status = "Invoiced";
            }

            if (order.OrderDetails.All(od => od.Status == "Invoiced"))
                order.Status = "Invoiced";

            _orderRepository.Update(order);

            return createdInvoices;
        }



        public InvoiceDto GetInvoiceForPartner(int invoiceId, int currentPartnerId)
        {
            var invoice = _invoices.GetByIdWithDetails(invoiceId);
            if (invoice == null)
                throw new Exception("Invoice not found");

            // Xác định loại hiển thị theo partner đang xem
            string displayType = (invoice.PartnerId == currentPartnerId)
                ? "Export"   // Bên phát hành (bán hàng)
                : "Import";  // Bên còn lại (mua hàng)

            // Map sang DTO trả về
            var dto = new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceCode = invoice.InvoiceCode,
                InvoiceType = displayType,   // hiển thị động
                PartnerId = invoice.PartnerId,
                PartnerName = invoice.Partner?.PartnerName ?? "N/A",
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                TotalAmount = invoice.TotalAmount,
                Status = invoice.Status,
                CreatedAt = invoice.CreatedAt
            };

            return dto;
        }
        public List<InvoiceDto> GetAllInvoicesForPartner(int partnerId)
        {
            var invoices = _invoices.GetAllWithDetails()
                .Where(i => i.PartnerId == partnerId || i.CreatedByNavigation.PartnerId == partnerId)
                .ToList();

            var result = new List<InvoiceDto>();

            foreach (var invoice in invoices)
            {
                // Đổi chiều hiển thị tùy bên đang xem
                string displayType;
                if (invoice.PartnerId == partnerId)
                    displayType = invoice.InvoiceType == "Export" ? "Import" : invoice.InvoiceType;
                else
                    displayType = invoice.InvoiceType == "Import" ? "Export" : invoice.InvoiceType;

                result.Add(new InvoiceDto
                {
                    InvoiceId = invoice.InvoiceId,
                    InvoiceCode = invoice.InvoiceCode,
                    InvoiceType = displayType,
                    PartnerId = invoice.PartnerId,
                    PartnerName = invoice.Partner?.PartnerName ?? "Không xác định",
                    IssueDate = invoice.IssueDate,
                    DueDate = invoice.DueDate,
                    TotalAmount = invoice.TotalAmount,
                    Status = invoice.Status,
                    CreatedAt = invoice.CreatedAt
                });
            }

            return result;
        }


    }
}