using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IInvoiceService
    {
        Invoice CreateInvoice(CreateInvoiceDto dto);
        Invoice? GetByIdWithDetails(int id);
        List<Invoice> GetAllWithDetails();
        Invoice? RejectInvoice(int id);
        List<Invoice> CreateInvoiceFromOrder(CreateInvoiceFromOrderDto dto);
        InvoiceDto GetInvoiceForPartner(int invoiceId, int currentPartnerId);
        List<InvoiceDto> GetAllInvoicesForPartner(int currentPartnerId);

    }
}