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

        public Invoice CreateInvoiceFromOrder(CreateInvoiceFromOrderDto dto)
        {
            var order = _orderRepository.GetByCode(dto.OrderCode);
            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status != "Approved")
                throw new Exception("Order must be approved to create invoice.");

            // Cập nhật UnitPrice từ DTO
            foreach (var od in order.OrderDetails)
            {
                var unitPriceDto = dto.UnitPrices?.FirstOrDefault(u => u.MaterialId == od.MaterialId);
                if (unitPriceDto != null)
                {
                    od.UnitPrice = unitPriceDto.UnitPrice;
                }

                if (!od.UnitPrice.HasValue)
                    throw new Exception($"UnitPrice not provided for MaterialId {od.MaterialId} in order.");
            }

            var invoice = new Invoice
            {
                InvoiceCode = $"INV-{_invoices.GetAllWithDetails().Count + 1:D3}",
                InvoiceType = "Order",
                PartnerId = order.CreatedBy ?? 0,
                CreatedBy = dto.CreatedBy,
                IssueDate = dto.IssueDate,
                DueDate = dto.DueDate,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            // Thêm chi tiết invoice
            foreach (var item in order.OrderDetails)
            {
                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice!.Value,
                    LineTotal = item.Quantity * item.UnitPrice!.Value
                });
            }

            invoice.TotalAmount = invoice.InvoiceDetails.Sum(d => d.LineTotal ?? 0);

            _invoices.Add(invoice);

            return invoice;
        }

    }
}