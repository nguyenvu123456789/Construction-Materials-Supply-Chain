using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services.Implements
{
    public class GlAccountService : IGlAccountService
    {
        private readonly IAccountRepository _repo;
        private readonly IMapper _mapper;

        public GlAccountService(IAccountRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public List<GlAccountDto> List(int partnerId, string? q, bool includeDeleted)
        {
            q = q?.Trim() ?? "";
            var query = _repo.QueryAll(includeDeleted).Where(x => x.PartnerId == partnerId);
            if (!string.IsNullOrEmpty(q)) query = query.Where(x => x.Code.Contains(q) || x.Name.Contains(q));
            return query.OrderBy(x => x.Code).ProjectTo<GlAccountDto>(_mapper.ConfigurationProvider).ToList();
        }

        public GlAccountDto? Get(int id, bool includeDeleted = false)
        {
            var entity = includeDeleted ? _repo.GetRawById(id) : _repo.GetById(id);
            if (entity == null) return null;
            return _mapper.Map<GlAccountDto>(entity);
        }

        public (bool ok, string? error, GlAccountDto? data) Create(GlAccountCreateDto dto)
        {
            if (dto.PartnerId <= 0) return (false, "PartnerId invalid", null);
            if (string.IsNullOrWhiteSpace(dto.Code)) return (false, "Code required", null);
            if (_repo.ExistsCode(dto.PartnerId, dto.Code, null)) return (false, "Code already exists", null);

            var entity = _mapper.Map<GlAccount>(dto);
            _repo.Add(entity);
            return (true, null, _mapper.Map<GlAccountDto>(entity));
        }

        public (bool ok, string? error, GlAccountDto? data) Update(int id, GlAccountUpdateDto dto)
        {
            var entity = _repo.GetById(id);
            if (entity == null) return (false, "Not found", null);

            _mapper.Map(dto, entity);
            _repo.Update(entity);
            return (true, null, _mapper.Map<GlAccountDto>(entity));
        }

        public (bool ok, string? error) SoftDelete(int id)
        {
            var entity = _repo.GetById(id);
            if (entity == null) return (false, "Not found");
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            _repo.Update(entity);
            return (true, null);
        }

        public (bool ok, string? error) Restore(int id)
        {
            var entity = _repo.GetRawById(id);
            if (entity == null) return (false, "Not found");
            if (!entity.IsDeleted) return (false, "Not deleted");
            if (_repo.ExistsCode(entity.PartnerId, entity.Code, entity.AccountId)) return (false, "Code conflict");
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            _repo.Update(entity);
            return (true, null);
        }
    }
}
