using Application.Common.Pagination;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IUserService
    {
        List<UserDto> GetAll();
        UserDto? GetById(int id);
        UserDto Create(UserCreateDto dto);
        void Update(int id, UserUpdateDto dto);
        void Delete(int id);
        PagedResultDto<UserDto> GetUsersFiltered(UserPagedQueryDto query);
    }
}
