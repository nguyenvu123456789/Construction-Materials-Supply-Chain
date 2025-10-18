using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IAuditLogService
    {
        List<AuditLog> GetFilteredRaw(string? searchTerm, int pageNumber, int pageSize, out int totalCount);
        List<AuditLogDto> GetFilteredDto(string? searchTerm, int pageNumber, int pageSize, out int totalCount);
        List<AuditLog> GetFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount);
    }
}
