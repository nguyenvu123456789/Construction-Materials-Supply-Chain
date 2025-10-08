using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IInvoiceService
    {
        Invoice CreateInvoice(CreateInvoiceDto dto);
        Invoice? GetById(int id);
        List<Invoice> GetAll();
    }
}
