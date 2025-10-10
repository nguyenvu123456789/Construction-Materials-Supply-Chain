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
    }
}
