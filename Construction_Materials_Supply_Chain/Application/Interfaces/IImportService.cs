using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IImportService
    {
        Import CreateImportFromInvoice(string importCode, string invoiceCode, int warehouseId, int createdBy, string? notes);
        Import ConfirmPendingImport(string importCode, string? notes);

        Import? GetById(int id);
        List<Import> GetAll();
        Import CreatePendingImport(int warehouseId, int createdBy, string? notes, List<PendingImportMaterialDto> materials);


    }
}
