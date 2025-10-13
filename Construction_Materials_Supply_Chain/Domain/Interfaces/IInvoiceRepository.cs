using Domain.Models;

namespace Domain.Interface
{
    public interface IInvoiceRepository
    {
        Invoice? GetByCode(string invoiceCode);
        Invoice? GetByIdWithDetails(int id);
        List<Invoice> GetAllWithDetails();
        void Add(Invoice entity);
        void Update(Invoice entity);
    }
}