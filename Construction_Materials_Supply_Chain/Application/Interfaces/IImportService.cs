using Domain.Models;

namespace Application.Interfaces
{
    public interface IImportService
    {
        Invoice ImportByCode(string invoiceCode, int warehouseId, int createdBy);
    }
}
