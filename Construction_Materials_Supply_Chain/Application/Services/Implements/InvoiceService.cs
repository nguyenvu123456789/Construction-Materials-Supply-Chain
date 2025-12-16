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
        private static int _invoiceCounter = 99;



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

            if (order.Status != StatusEnum.Approved.ToStatusString()
                && order.Status != StatusEnum.Invoiced.ToStatusString())
            {
                throw new Exception(InvoiceMessages.ORDER_NOT_APPROVED);
            }

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

            var relation = _partnerRelationRepository.GetRelation(
                buyerPartnerId: partnerId.Value,
                sellerPartnerId: order.CreatedByNavigation.PartnerId.Value
            );

            decimal discountPercent = relation?.RelationType.DiscountPercent ?? 0;
            decimal discountAmount = relation?.RelationType.DiscountAmount ?? 0;

            var invoice = new Invoice
            {
                InvoiceCode = GenerateInvoiceCode(),
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

            var (totalAmount, totalDiscount) = CalculateTotals(selectedDetails, dto, invoice);

            invoice.TotalAmount = totalAmount;
            invoice.DiscountAmount = totalDiscount;
            invoice.PayableAmount = totalAmount - totalDiscount;

            _invoices.Add(invoice);
            createdInvoices.Add(invoice);

            // 1. Update status cho các OrderDetail đã được invoiced
            foreach (var detail in selectedDetails)
            {
                detail.Status = StatusEnum.Invoiced.ToStatusString();
            }

            // 2. Nếu tất cả OrderDetail đều invoiced → update Order
            if (order.OrderDetails.All(od => od.Status == StatusEnum.Invoiced.ToStatusString()))
            {
                order.Status = StatusEnum.Invoiced.ToStatusString();
            }

            _orderRepository.Update(order);

            return createdInvoices;
        }

        private (decimal totalAmount, decimal totalDiscount) CalculateTotals(
            List<OrderDetail> selectedDetails,
            CreateInvoiceFromOrderDto dto,
            Invoice invoice)
        {
            decimal totalAmount = 0;
            decimal totalDiscount = 0;

            foreach (var od in selectedDetails)
            {
                var unitPriceDto = dto.UnitPrices.FirstOrDefault(u => u.MaterialId == od.MaterialId);
                if (unitPriceDto == null) continue;

                int deliveredQty = unitPriceDto.DeliveredQuantity;
                int deliveredBefore = od.DeliveredQuantity;

                if (deliveredBefore + deliveredQty > od.Quantity)
                    throw new Exception(string.Format(InvoiceMessages.DELIVERED_QTY_EXCEEDS_ORDER, od.MaterialId));

                od.DeliveredQuantity = deliveredBefore + deliveredQty;

                decimal finalPrice = od.FinalPrice;
                decimal unitPrice = od.UnitPrice ?? 0m;

                decimal lineTotal = deliveredQty * finalPrice;
                decimal lineDiscount = (unitPrice - finalPrice) * deliveredQty;
                if (lineDiscount < 0) lineDiscount = 0m;

                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    MaterialId = od.MaterialId,
                    Quantity = deliveredQty,
                    UnitPrice = unitPrice,
                    LineTotal = lineTotal,
                    DiscountAmount = lineDiscount
                });

                totalAmount += lineTotal;
                totalDiscount += lineDiscount;

                od.Status = od.DeliveredQuantity == od.Quantity
                    ? StatusEnum.Success.ToStatusString()
                    : StatusEnum.Invoiced.ToStatusString();
            }

            return (totalAmount, totalDiscount);
        }
        private string GenerateInvoiceCode()
        {
            int next = Interlocked.Increment(ref _invoiceCounter);
            return $"INV-{next}";
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


        public void MarkInvoicesAsDelivered(List<int> invoiceIds)
        {
            if (invoiceIds == null || !invoiceIds.Any())
                throw new Exception("Danh sách hóa đơn rỗng");

            foreach (var invoiceId in invoiceIds)
            {
                var invoice = _invoices.GetById(invoiceId);
                if (invoice == null)
                    throw new Exception($"Không tìm thấy hóa đơn ID = {invoiceId}");

                invoice.ExportStatus = StatusEnum.Completed.ToStatusString(); // Delivered
                invoice.UpdatedAt = DateTime.Now;

                _invoices.Update(invoice);
            }
        }


        //  Lấy hóa đơn theo Partner 
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

        // Lấy tất cả hóa đơn theo Partner 
        public List<InvoiceDto> GetAllInvoicesForPartnerOrManager(int? partnerId, int? managerId)
        {
            var invoicesQuery = _invoices.GetAllWithDetails()
                .AsQueryable();

            if (partnerId.HasValue || managerId.HasValue)
            {
                invoicesQuery = invoicesQuery.Where(i =>
                    (partnerId.HasValue && (i.PartnerId == partnerId.Value
                                           || i.CreatedByNavigation != null
                                           && i.CreatedByNavigation.PartnerId == partnerId.Value)) ||
                    (managerId.HasValue && i.Warehouse != null && i.Warehouse.ManagerId == managerId.Value)
                );
            }

            var result = invoicesQuery.Select(invoice => new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceCode = invoice.InvoiceCode,
                InvoiceType = invoice.CreatedByNavigation != null && invoice.CreatedByNavigation.PartnerId == partnerId
                                ? "Export" : "Import",
                PartnerId = invoice.PartnerId,
                PartnerName = invoice.Partner != null ? invoice.Partner.PartnerName : "Không xác định",
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                TotalAmount = invoice.TotalAmount,
                Address = invoice.Address,
                DiscountAmount = invoice.DiscountAmount,
                PayableAmount = invoice.PayableAmount,
                WarehouseId = invoice.WarehouseId,
                WarehouseName = invoice.Warehouse != null ? invoice.Warehouse.WarehouseName : "Không xác định",
                Status = invoice.CreatedByNavigation != null && invoice.CreatedByNavigation.PartnerId == partnerId
                            ? invoice.ExportStatus : invoice.ImportStatus,
                CreatedAt = invoice.CreatedAt
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
