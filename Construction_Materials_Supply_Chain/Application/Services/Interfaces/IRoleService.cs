using Application.DTOs;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        List<RoleDto> GetAll();
        RoleDto? GetById(int id);
        RoleDto Create(RoleCreateDto dto);
        void Update(int id, RoleUpdateDto dto);
        void Delete(int id);
    }
}
