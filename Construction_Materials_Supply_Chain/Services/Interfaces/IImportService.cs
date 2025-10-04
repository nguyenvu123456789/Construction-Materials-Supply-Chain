using BusinessObjects;

namespace Services.Interfaces
{
    public interface IImportService
    {
        Invoice ImportByCode(string invoiceCode, int warehouseId, int createdBy);
    }
}
