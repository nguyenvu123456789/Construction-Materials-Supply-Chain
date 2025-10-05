using BusinessObjects;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Implementations
{
    public class ImportService : IImportService
    {
        private readonly IImportRepository _repo;

        public ImportService(IImportRepository repo)
        {
            _repo = repo;
        }

        public Invoice ImportByCode(string invoiceCode, int warehouseId, int createdBy)
        {
            var invoice = _repo.GetPendingInvoiceByCode(invoiceCode);
            if (invoice == null)
                throw new InvalidOperationException($"Invoice {invoiceCode} không tồn tại hoặc không ở trạng thái Pending.");

            _repo.ImportInvoice(invoice, warehouseId, createdBy);
            return invoice;
        }
    }
}
