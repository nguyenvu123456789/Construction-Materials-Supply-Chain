using Application.Common.Pagination;
using Application.DTOs;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface IUserService
    {
        List<UserDto> GetAll();
        UserDto? GetById(int id);
        UserDto Create(UserCreateDto dto);
        void Update(int id, UserUpdateDto dto);
        void Delete(int id);
        PagedResultDto<UserDto> GetUsersFiltered(UserPagedQueryDto query, List<string>? statuses = null);
        PagedResultDto<UserDto> GetUsersFilteredIncludeDeleted(UserPagedQueryDto query, List<string>? statuses = null);
        void Restore(int id, string status);
    }
}
