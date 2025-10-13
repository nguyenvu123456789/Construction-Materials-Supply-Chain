using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Interface;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repo;
    private readonly IMapper _mapper;

    public AuditLogService(IAuditLogRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public List<AuditLog> GetFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount)
    {
        var q = _repo.QueryWithUser().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            q = q.Where(a =>
                (a.EntityName ?? "").Contains(term) ||
                (a.Action ?? "").Contains(term) ||
                (a.Changes ?? "").Contains(term) ||
                (a.User != null && ((a.User.UserName ?? "").Contains(term) || (a.User.FullName ?? "").Contains(term))));
        }

        totalCount = q.Count();

        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        return q.OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
    }

    public List<AuditLogDto> GetFilteredDto(string? searchTerm, int pageNumber, int pageSize, out int totalCount)
    {
        var q = _repo.QueryWithUser().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            q = q.Where(a =>
                (a.EntityName ?? "").Contains(term) ||
                (a.Action ?? "").Contains(term) ||
                (a.Changes ?? "").Contains(term) ||
                (a.User != null && ((a.User.UserName ?? "").Contains(term) || (a.User.FullName ?? "").Contains(term))));
        }

        totalCount = q.Count();

        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        return q.OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<AuditLogDto>(_mapper.ConfigurationProvider)
                .ToList();
    }

    public List<AuditLog> GetFilteredRaw(string? searchTerm, int pageNumber, int pageSize, out int totalCount)
        => GetFiltered(searchTerm, pageNumber, pageSize, out totalCount);
}
