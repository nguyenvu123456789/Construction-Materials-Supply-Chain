using Application.Constants.Enums;
using Application.Constants.Messages;
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
        private readonly IOrderRepository _orderRepository;
        private readonly IPartnerRelationRepository _partnerRelationRepository;


        public InvoiceService(
            IInvoiceRepository invoices,
            IMaterialRepository materials,
            IPartnerRelationRepository partnerRelationRepository,
            IOrderRepository orderRepository)
        {
            _invoices = invoices;
            _materials = materials;
            _orderRepository = orderRepository;
            _partnerRelationRepository = partnerRelationRepository;
        }

        public Invoice CreateInvoice(CreateInvoiceDto dto)
        {
            if (_invoices.GetByCode(dto.InvoiceCode) != null)
                throw new Exception(InvoiceMessages.INVOICE_CODE_EXISTS);

            var invoice = new Invoice
            {
                InvoiceCode = dto.InvoiceCode,
                InvoiceType = dto.InvoiceType,
                PartnerId = dto.PartnerId,
                CreatedBy = dto.CreatedBy,
                IssueDate = dto.IssueDate,
                DueDate = dto.DueDate,
                ExportStatus = StatusEnum.Pending.ToStatusString(),
                ImportStatus = StatusEnum.Pending.ToStatusString(),
                CreatedAt = DateTime.Now
            };

            foreach (var item in dto.Details)
            {
                var material = _materials.GetById(item.MaterialId);
                if (material == null)
                    throw new Exception(string.Format(InvoiceMessages.MATERIAL_NOT_FOUND, item.MaterialId));

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


        public List<Invoice> CreateInvoiceFromOrder(CreateInvoiceFromOrderDto dto)
        {
            var order = _orderRepository.GetByCode(dto.OrderCode);
            if (order == null)
                throw new Exception(InvoiceMessages.ORDER_NOT_FOUND);

            if (order.Status != StatusEnum.Approved.ToStatusString())
                throw new Exception(InvoiceMessages.ORDER_NOT_APPROVED);

            var partnerId = order.CreatedByNavigation?.PartnerId;
            if (partnerId == null || partnerId == 0)
                throw new Exception(InvoiceMessages.PARTNER_NOT_FOUND);

            if (dto.UnitPrices == null || !dto.UnitPrices.Any())
                throw new Exception(InvoiceMessages.NO_MATERIAL_PROVIDED);

            var selectedMaterialIds = dto.UnitPrices.Select(u => u.MaterialId).ToList();
            var selectedDetails = order.OrderDetails
                .Where(od => selectedMaterialIds.Contains(od.MaterialId))
                .ToList();

            if (!selectedDetails.Any())
                throw new Exception(InvoiceMessages.NO_MATCHING_MATERIALS);

            var createdInvoices = new List<Invoice>();

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
                throw new Exception(InvoiceMessages.PARTNER_NOT_FOUND);

            var relation = _partnerRelationRepository.GetRelation(
                buyerPartnerId: partnerId.Value,
                sellerPartnerId: order.CreatedByNavigation.PartnerId.Value
            );

            decimal discountPercent = relation?.RelationType.DiscountPercent ?? 0;
            decimal discountAmount = relation?.RelationType.DiscountAmount ?? 0;

            var newCode = $"INV-{nextNumber:D3}";

            var invoice = new Invoice
            {
                InvoiceCode = newCode,
                InvoiceType = StatusEnum.Export.ToStatusString(),
                PartnerId = partnerId.Value,
                WarehouseId = order.WarehouseId,
                CreatedBy = dto.CreatedBy,
                OrderId = order.OrderId,
                IssueDate = dto.IssueDate,
                DueDate = dto.DueDate,
                ExportStatus = StatusEnum.Pending.ToStatusString(),
                ImportStatus = StatusEnum.Pending.ToStatusString(),
                CreatedAt = DateTime.Now,
                Address = order.DeliveryAddress
            };

            decimal totalAmount = 0;
            decimal totalDiscount = 0;

            foreach (var od in selectedDetails)
            {
                var unitPriceDto = dto.UnitPrices.FirstOrDefault(u => u.MaterialId == od.MaterialId);
                if (unitPriceDto == null) continue;

                var unitPrice = unitPriceDto.UnitPrice;
                var lineTotal = od.Quantity * unitPrice;

                decimal lineDiscount = lineTotal * discountPercent / 100 + discountAmount;
                if (lineDiscount > lineTotal) lineDiscount = lineTotal;

                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    MaterialId = od.MaterialId,
                    Quantity = od.Quantity,
                    UnitPrice = unitPrice,
                    LineTotal = lineTotal,
                    DiscountAmount = lineDiscount
                });

                totalAmount += lineTotal;
                totalDiscount += lineDiscount;
                od.UnitPrice = unitPrice;
                od.Status = StatusEnum.Invoiced.ToStatusString();
            }

            invoice.TotalAmount = totalAmount;
            invoice.DiscountAmount = totalDiscount;
            invoice.PayableAmount = totalAmount - totalDiscount;

            _invoices.Add(invoice);
            createdInvoices.Add(invoice);

            if (order.OrderDetails.All(od => od.Status == StatusEnum.Invoiced.ToStatusString()))
                order.Status = StatusEnum.Invoiced.ToStatusString();

            _orderRepository.Update(order);

            return createdInvoices;
        }

        public Invoice? UpdateExportStatus(int id, string newStatus)
        {
            var invoice = _invoices.GetByIdWithDetails(id);
            if (invoice == null)
                throw new Exception(InvoiceMessages.INVOICE_NOT_FOUND);

            invoice.ExportStatus = newStatus;
            invoice.UpdatedAt = DateTime.Now;
            _invoices.Update(invoice);
            return invoice;
        }

        public Invoice? UpdateImportStatus(int id, string newStatus)
        {
            var invoice = _invoices.GetByIdWithDetails(id);
            if (invoice == null)
                throw new Exception(InvoiceMessages.INVOICE_NOT_FOUND);

            invoice.ImportStatus = newStatus;
            invoice.UpdatedAt = DateTime.Now;
            _invoices.Update(invoice);
            return invoice;
        }

        // 🔹 Lấy hóa đơn theo Partner 
        public InvoiceDto GetInvoiceForPartner(int invoiceId, int currentPartnerId)
        {
            var invoice = _invoices.GetByIdWithDetails(invoiceId);
            if (invoice == null)
                throw new Exception(InvoiceMessages.INVOICE_NOT_FOUND);

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
                WarehouseId = invoice.WarehouseId,
                WarehouseName = invoice.Warehouse?.WarehouseName ?? "Không xác định",
                Status = isExporter ? invoice.ExportStatus : invoice.ImportStatus,
                CreatedAt = invoice.CreatedAt
            };

            return dto;
        }

        // 🔹 Lấy tất cả hóa đơn theo Partner 
        public List<InvoiceDto> GetAllInvoicesForPartnerOrManager(int? partnerId, int? managerId)
        {
            var invoices = _invoices.GetAllWithDetails();

            if (partnerId.HasValue)
                invoices = invoices.Where(i => i.PartnerId == partnerId.Value || i.CreatedByNavigation?.PartnerId == partnerId.Value).ToList();

            if (managerId.HasValue)
                invoices = invoices.Where(i => i.Warehouse != null && i.Warehouse.ManagerId == managerId.Value).ToList();

            var result = invoices.Select(invoice =>
            {
                bool isExporter = invoice.CreatedByNavigation?.PartnerId == partnerId;

                return new InvoiceDto
                {
                    InvoiceId = invoice.InvoiceId,
                    InvoiceCode = invoice.InvoiceCode,
                    InvoiceType = isExporter ? "Export" : "Import",
                    PartnerId = invoice.PartnerId,
                    PartnerName = invoice.Partner?.PartnerName ?? "Không xác định",
                    IssueDate = invoice.IssueDate,
                    DueDate = invoice.DueDate,
                    TotalAmount = invoice.TotalAmount,
                    Address = invoice.Address,
                    DiscountAmount = invoice.DiscountAmount,
                    PayableAmount = invoice.PayableAmount,
                    WarehouseId = invoice.WarehouseId,
                    WarehouseName = invoice.Warehouse?.WarehouseName ?? "Không xác định",
                    Status = isExporter ? invoice.ExportStatus : invoice.ImportStatus,
                    CreatedAt = invoice.CreatedAt
                };
            }).ToList();

            return result;
        }


        public Invoice? RejectInvoice(int id)
        {
            var invoice = _invoices.GetByIdWithDetails(id);
            if (invoice == null)
                return null;

            invoice.ExportStatus = StatusEnum.Rejected.ToStatusString();
            invoice.ImportStatus = StatusEnum.Rejected.ToStatusString();
            invoice.UpdatedAt = DateTime.Now;
            _invoices.Update(invoice);

            return invoice;
        }

        public Invoice? GetByIdWithDetails(int id) => _invoices.GetByIdWithDetails(id);
        public List<Invoice> GetAllWithDetails() => _invoices.GetAllWithDetails();

    }
}
