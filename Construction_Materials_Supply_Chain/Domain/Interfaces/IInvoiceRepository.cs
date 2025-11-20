using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Invoice? GetByCode(string invoiceCode);
        Invoice? GetByIdWithDetails(int id);
        List<Invoice> GetAllWithDetails();
        void Add(Invoice entity);
        void Update(Invoice entity);
        void AddInvoiceDetail(InvoiceDetail detail);
        List<InvoiceDetail> GetInvoiceDetailsByInvoiceId(int invoiceId);
        List<Invoice> GetAll();
        Invoice GetWithDetails(int id);
        Invoice? GetWithDetailsByCode(string code);
        List<Invoice> GetCustomerImportInvoices(int customerPartnerId);
        List<Invoice> GetExportInvoicesByOrderIds(List<int> orderIds);
    }
}