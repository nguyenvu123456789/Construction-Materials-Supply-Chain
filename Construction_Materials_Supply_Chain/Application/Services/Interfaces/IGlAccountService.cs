using Application.DTOs;

namespace Application.Services.Interfaces
{
    public interface IGlAccountService
    {
        List<GlAccountDto> List(int partnerId, string? q, bool includeDeleted);
        GlAccountDto? Get(int id, bool includeDeleted = false);
        (bool ok, string? error, GlAccountDto? data) Create(GlAccountCreateDto dto);
        (bool ok, string? error, GlAccountDto? data) Update(int id, GlAccountUpdateDto dto);
        (bool ok, string? error) SoftDelete(int id);
        (bool ok, string? error) Restore(int id);
    }
}
