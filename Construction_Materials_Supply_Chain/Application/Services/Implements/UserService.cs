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
    private readonly IRoleRepository _roles;
    private readonly IMapper _mapper;

    public UserService(IUserRepository users, IRoleRepository roles, IMapper mapper)
    {
        _users = users;
        _roles = roles;
        _mapper = mapper;
    }

    public List<UserDto> GetAll() =>
        _users.QueryWithRoles()
              .AsNoTracking()
              .Where(u => u.Status != "Deleted")
              .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
              .ToList();

    public UserDto? GetById(int id) =>
        _users.QueryWithRoles()
              .AsNoTracking()
              .Where(u => u.UserId == id && u.Status != "Deleted")
              .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
              .FirstOrDefault();

    public List<UserDto> GetAllWithRoles() =>
        _users.QueryWithRoles()
              .Where(u => u.Status != "Deleted")
              .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
              .ToList();

    public UserDto? GetByIdWithRoles(int id) =>
        _users.QueryWithRoles()
              .Where(u => u.UserId == id && u.Status != "Deleted")
              .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
              .FirstOrDefault();

    public UserDto Create(UserCreateDto dto)
    {
        var entity = _mapper.Map<User>(dto);
        entity.CreatedAt = DateTime.UtcNow;
        entity.Status = "Active";

        entity.AvatarBase64 = NormalizeBase64(dto.AvatarBase64);

        _users.Add(entity);

        var created = _users.QueryWithRolesIncludeDeleted().First(u => u.UserId == entity.UserId);
        if (dto.RoleIds != null && dto.RoleIds.Any())
            _users.AssignRoles(entity.UserId, dto.RoleIds);

        return _mapper.Map<UserDto>(created);
    }

    public void Update(int id, UserUpdateDto dto)
    {
        var existing = _users.QueryWithRolesIncludeDeleted().FirstOrDefault(u => u.UserId == id);
        if (existing == null) throw new KeyNotFoundException("User not found");
        if (existing.Status == "Deleted") throw new InvalidOperationException("Cannot update deleted user");

        _mapper.Map(dto, existing);

        if (dto.AvatarBase64 != null)
            existing.AvatarBase64 = NormalizeBase64(dto.AvatarBase64);

        existing.UpdatedAt = DateTime.UtcNow;
        _users.Update(existing);

        if (dto.RoleIds != null)
            _users.AssignRoles(existing.UserId, dto.RoleIds);
    }

    public void Delete(int id)
    {
        var u = _users.GetById(id);
        if (u == null) return;
        if (u.Status == "Deleted") return;
        u.Status = "Deleted";
        u.UpdatedAt = DateTime.UtcNow;
        _users.Update(u);
    }

    public void Restore(int id, string status)
    {
        var s = (status ?? string.Empty).Trim();
        if (!string.Equals(s, "Active", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(s, "Inactive", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Status must be Active or Inactive");

        var u = _users.QueryWithRolesIncludeDeleted().FirstOrDefault(x => x.UserId == id);
        if (u == null) throw new KeyNotFoundException("User not found");
        if (u.Status != "Deleted") return;
        u.Status = char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
        u.UpdatedAt = DateTime.UtcNow;
        _users.Update(u);
    }

    public PagedResultDto<UserDto> GetUsersFiltered(UserPagedQueryDto query, List<string>? statuses = null)
    {
        var q = _users.QueryWithRoles().AsNoTracking().Where(u => u.Status != "Deleted");
        if (statuses != null && statuses.Count > 0)
        {
            var set = new HashSet<string>(statuses.Where(s => !string.IsNullOrWhiteSpace(s))
                                                  .Select(s => s.Trim().ToLowerInvariant()));
            set.Remove("deleted");
            if (set.Count > 0)
                q = q.Where(u => set.Contains(u.Status.ToLower()));
        }
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
            var rset = query.Roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim().ToLowerInvariant())
                .ToHashSet();
            q = q.Where(u => u.UserRoles.Any(ur =>
                rset.Contains(ur.Role.RoleName.ToLower()) ||
                rset.Contains(ur.Role.RoleId.ToString())
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

    public PagedResultDto<UserDto> GetUsersFilteredIncludeDeleted(UserPagedQueryDto query, List<string>? statuses = null)
    {
        var q = _users.QueryWithRolesIncludeDeleted().AsNoTracking();
        if (statuses != null && statuses.Count > 0)
        {
            var set = new HashSet<string>(statuses
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToLowerInvariant()));
            if (set.Count > 0)
                q = q.Where(u => set.Contains(u.Status.ToLower()));
        }
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
            var rset = query.Roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim().ToLowerInvariant())
                .ToHashSet();
            q = q.Where(u => u.UserRoles.Any(ur =>
                rset.Contains(ur.Role.RoleName.ToLower()) ||
                rset.Contains(ur.Role.RoleId.ToString())
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

    private static string? NormalizeBase64(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        var s = input.Trim();

        var comma = s.IndexOf(',');
        if (comma >= 0)
            s = s[(comma + 1)..];

        try
        {
            Convert.FromBase64String(s);
            return s;
        }
        catch
        {
            throw new ArgumentException("AvatarBase64 is invalid base64 data.");
        }
    }

    public UserDto? GetByIdWithPartner(int id)
    {
        var user = _users.GetByIdWithPartner(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }
}
