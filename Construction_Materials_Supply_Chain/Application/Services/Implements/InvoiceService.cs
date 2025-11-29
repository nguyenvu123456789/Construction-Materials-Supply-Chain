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
        private readonly IPartnerRelationRepository _partnerRelationRepository;


        public InvoiceService(
            IInvoiceRepository invoices,
            IMaterialRepository materials,
            IInventoryRepository inventories,
            IImportRepository imports,
            IPartnerRelationRepository partnerRelationRepository,
            IOrderRepository orderRepository)
        {
            _invoices = invoices;
            _materials = materials;
            _inventories = inventories;
            _imports = imports;
            _orderRepository = orderRepository;
            _partnerRelationRepository = partnerRelationRepository;
        }

        // 🔹 Tạo hóa đơn thủ công
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
                ExportStatus = "Pending",
                ImportStatus = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in dto.Details)
            {
                var material = _materials.GetById(item.MaterialId);
                if (material == null)
                    throw new Exception($"MaterialId {item.MaterialId} not found.");

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

        // 🔹 Lấy 1 hóa đơn theo ID kèm chi tiết
        public Invoice? GetByIdWithDetails(int id) => _invoices.GetByIdWithDetails(id);

        // 🔹 Lấy tất cả hóa đơn có chi tiết
        public List<Invoice> GetAllWithDetails() => _invoices.GetAllWithDetails();

        // 🔹 Tạo hóa đơn từ Order
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

            var selectedMaterialIds = dto.UnitPrices.Select(u => u.MaterialId).ToList();
            var selectedDetails = order.OrderDetails
                .Where(od => selectedMaterialIds.Contains(od.MaterialId))
                .ToList();

            if (!selectedDetails.Any())
                throw new Exception("No matching materials found in the order.");

            var createdInvoices = new List<Invoice>();

            // 🔹 Sinh mã hóa đơn mới tự động
            var lastInvoice = _invoices.GetAllWithDetails()
                .OrderByDescending(i => i.InvoiceId)
                .FirstOrDefault();

            int nextNumber = 1;
            if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceCode))
            {
                var parts = lastInvoice.InvoiceCode.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                    nextNumber = lastNumber + 1;
            }

            if (partnerId == null || order.CreatedByNavigation?.PartnerId == null)
                throw new Exception("PartnerId (buyer or seller) not found for this order.");

            var relation = _partnerRelationRepository.GetRelation(
                buyerPartnerId: partnerId.Value,
                sellerPartnerId: order.CreatedByNavigation.PartnerId.Value
            );

            decimal discountPercent = relation?.RelationType.DiscountPercent ?? 0;
            decimal discountAmount = relation?.RelationType.DiscountAmount ?? 0;

            foreach (var od in selectedDetails)
            {
                var unitPriceDto = dto.UnitPrices.FirstOrDefault(u => u.MaterialId == od.MaterialId);
                if (unitPriceDto == null)
                    continue;

                var unitPrice = unitPriceDto.UnitPrice;
                var lineTotal = od.Quantity * unitPrice;

                // 🔹 Áp dụng giảm giá toàn bộ hóa đơn
                decimal lineDiscount = lineTotal * discountPercent / 100 + discountAmount;

                // Không được giảm vượt quá tổng line
                if (lineDiscount > lineTotal)
                    lineDiscount = lineTotal;

                var newCode = $"INV-{nextNumber:D3}";
                nextNumber++;

                var invoice = new Invoice
                {
                    InvoiceCode = newCode,
                    InvoiceType = "Export",
                    PartnerId = partnerId.Value,
                    CreatedBy = dto.CreatedBy,
                    OrderId = order.OrderId,
                    IssueDate = dto.IssueDate,
                    DueDate = dto.DueDate,
                    ExportStatus = "Pending",
                    ImportStatus = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    TotalAmount = lineTotal,
                    DiscountAmount = lineDiscount,
                    PayableAmount = lineTotal - lineDiscount
                };

                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    MaterialId = od.MaterialId,
                    Quantity = od.Quantity,
                    UnitPrice = unitPrice,
                    LineTotal = lineTotal,
                    DiscountAmount = lineDiscount
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


        // 🔹 Cập nhật trạng thái của bên xuất
        public Invoice? UpdateExportStatus(int id, string newStatus)
        {
            var invoice = _invoices.GetByIdWithDetails(id);
            if (invoice == null)
                throw new Exception("Invoice not found.");

            invoice.ExportStatus = newStatus;
            invoice.UpdatedAt = DateTime.UtcNow;
            _invoices.Update(invoice);
            return invoice;
        }

        // 🔹 Cập nhật trạng thái của bên nhập
        public Invoice? UpdateImportStatus(int id, string newStatus)
        {
            var invoice = _invoices.GetByIdWithDetails(id);
            if (invoice == null)
                throw new Exception("Invoice not found.");

            invoice.ImportStatus = newStatus;
            invoice.UpdatedAt = DateTime.UtcNow;
            _invoices.Update(invoice);
            return invoice;
        }

        // 🔹 Lấy hóa đơn theo Partner (phân biệt bên xem)
        public InvoiceDto GetInvoiceForPartner(int invoiceId, int currentPartnerId)
        {
            var invoice = _invoices.GetByIdWithDetails(invoiceId);
            if (invoice == null)
                throw new Exception("Invoice not found");

            bool isExporter = invoice.CreatedByNavigation?.PartnerId == currentPartnerId;

            var dto = new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceCode = invoice.InvoiceCode,
                InvoiceType = isExporter ? "Export" : "Import",
                PartnerId = invoice.PartnerId,
                PartnerName = invoice.Partner?.PartnerName ?? "N/A",
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                TotalAmount = invoice.TotalAmount,
                Status = isExporter ? invoice.ExportStatus : invoice.ImportStatus,
                CreatedAt = invoice.CreatedAt
            };

            return dto;
        }

        // 🔹 Lấy tất cả hóa đơn theo Partner (phân biệt vai trò)
        public List<InvoiceDto> GetAllInvoicesForPartner(int partnerId)
        {
            var invoices = _invoices.GetAllWithDetails()
                .Where(i => i.PartnerId == partnerId || i.CreatedByNavigation.PartnerId == partnerId)
                .ToList();

            var result = new List<InvoiceDto>();

            foreach (var invoice in invoices)
            {
                bool isExporter = invoice.CreatedByNavigation.PartnerId == partnerId;

                result.Add(new InvoiceDto
                {
                    InvoiceId = invoice.InvoiceId,
                    InvoiceCode = invoice.InvoiceCode,
                    InvoiceType = isExporter ? "Export" : "Import",
                    PartnerId = invoice.PartnerId,
                    PartnerName = invoice.Partner?.PartnerName ?? "Không xác định",
                    IssueDate = invoice.IssueDate,
                    DueDate = invoice.DueDate,
                    TotalAmount = invoice.TotalAmount,
                    DiscountAmount = invoice.DiscountAmount,
                    PayableAmount = invoice.PayableAmount,
                    Status = isExporter ? invoice.ExportStatus : invoice.ImportStatus,
                    CreatedAt = invoice.CreatedAt
                });
            }

            return result;
        }
        public Invoice? RejectInvoice(int id)
        {
            var invoice = _invoices.GetByIdWithDetails(id);
            if (invoice == null)
                return null;

            invoice.ExportStatus = "Rejected";
            invoice.ImportStatus = "Rejected";
            invoice.UpdatedAt = DateTime.UtcNow;
            _invoices.Update(invoice);

            return invoice;
        }

    }
}
