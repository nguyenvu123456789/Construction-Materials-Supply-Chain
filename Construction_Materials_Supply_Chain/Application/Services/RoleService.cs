using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Interface;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roles;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository roles, IMapper mapper)
    {
        _roles = roles;
        _mapper = mapper;
    }

    public List<RoleDto> GetAll()
        => _roles.Query()
                 .AsNoTracking()
                 .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
                 .ToList();

    public RoleDto? GetById(int id)
    {
        var role = _roles.Query()
                         .AsNoTracking()
                         .FirstOrDefault(r => r.RoleId == id);
        return role == null ? null : _mapper.Map<RoleDto>(role);
    }

    public RoleDto Create(RoleCreateDto dto)
    {
        var entity = _mapper.Map<Role>(dto);
        _roles.Add(entity);
        var created = _roles.Query().AsNoTracking().First(r => r.RoleId == entity.RoleId);
        return _mapper.Map<RoleDto>(created);
    }

    public void Update(int id, RoleUpdateDto dto)
    {
        var entity = _roles.GetById(id);
        if (entity == null) throw new KeyNotFoundException();
        _mapper.Map(dto, entity);
        _roles.Update(entity);
    }

    public void Delete(int id)
    {
        var entity = _roles.GetById(id);
        if (entity == null) return;
        _roles.Delete(entity);
    }
}
