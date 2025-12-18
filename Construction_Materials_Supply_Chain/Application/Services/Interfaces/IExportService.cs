using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IExportService
    {
        Export CreatePendingExport(ExportRequestDto dto);
        Export ConfirmExport(string exportCode, string? notes);
        ExportResponseDto? GetById(int id);
        List<ExportResponseDto> GetAllExports();
        List<ExportResponseDto> GetExportsByPartnerOrManager(int? partnerId = null, int? managerId = null);
        Export? RejectExport(int id);
        Export CreateExportFromInvoice(ExportFromInvoiceDto dto);
    }
}