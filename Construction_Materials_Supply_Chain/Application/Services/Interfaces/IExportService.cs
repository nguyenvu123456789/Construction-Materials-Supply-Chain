using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IExportService
    {
        Export CreatePendingExport(ExportRequestDto dto);
        Export ConfirmExport(string exportCode, string? notes);
        Export? GetById(int id);
        List<Export> GetAll();
        List<Export> GetByPartnerOrManager(int? partnerId = null, int? managerId = null);
        Export? RejectExport(int id);
        Export CreateExportFromInvoice(ExportFromInvoiceDto dto);
    }
}