using Application.Common.Pagination;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Interface;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;

    public UserService(IUserRepository users, IMapper mapper)
    {
        _users = users;
        _mapper = mapper;
    }

    public List<UserDto> GetAll()
        => _users.QueryWithRoles()
                 .AsNoTracking()
                 .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                 .ToList();

    public UserDto? GetById(int id)
        => _users.QueryWithRoles()
                 .AsNoTracking()
                 .Where(u => u.UserId == id)
                 .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                 .FirstOrDefault();

    public List<UserDto> GetAllWithRoles()
        => _users.QueryWithRoles()
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider).ToList();

    public UserDto? GetByIdWithRoles(int id)
        => _users.QueryWithRoles()
            .Where(u => u.UserId == id)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefault();

    public UserDto Create(UserCreateDto dto)
    {
        var entity = _mapper.Map<User>(dto);
        entity.CreatedAt = DateTime.UtcNow;

        _users.Add(entity);

        var created = _users.QueryWithRoles().First(u => u.UserId == entity.UserId);
        return _mapper.Map<UserDto>(created);
    }

    public void Update(int id, UserUpdateDto dto)
    {
        var existing = _users.QueryWithRoles().FirstOrDefault(u => u.UserId == id);
        if (existing == null) throw new KeyNotFoundException("User not found");

        _mapper.Map(dto, existing);

        _users.Update(existing);
    }

    public void Delete(int id)
    {
        var u = _users.GetById(id);
        if (u != null) _users.Delete(u);
    }

    public PagedResultDto<UserDto> GetUsersFiltered(UserPagedQueryDto query)
    {
        var q = _users.QueryWithRoles().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim();
            q = q.Where(u =>
                (u.UserName ?? "").Contains(term) ||
                (u.Email ?? "").Contains(term) ||
                (u.FullName ?? "").Contains(term));
        }

        if (query.Roles != null && query.Roles.Count > 0)
        {
            var set = query.Roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim().ToLowerInvariant())
                .ToHashSet();

            q = q.Where(u => u.UserRoles.Any(ur =>
                set.Contains(ur.Role.RoleName.ToLower()) ||
                set.Contains(ur.Role.RoleId.ToString())
            ));
        }

        var pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
        var pageSize = query.PageSize > 0 ? query.PageSize : 10;

        var totalCount = q.Count();

        var data = q
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToList();

        return new PagedResultDto<UserDto>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
