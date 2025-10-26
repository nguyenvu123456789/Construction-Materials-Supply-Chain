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
        Invoice CreateInvoiceFromOrder(CreateInvoiceFromOrderDto dto);
        List<Invoice> GetByType(string type);
        InvoiceDto GetInvoiceForPartner(int invoiceId, int currentPartnerId);
        List<InvoiceDto> GetAllInvoicesForPartner(int currentPartnerId);

    }
}