using Application.Common.Pagination;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using FluentValidation;

namespace Services.Implementations
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _repo;
        private readonly IMapper _mapper;
        private readonly IValidator<ActivityLogPagedQueryDto> _validator;

        public ActivityLogService(
            IActivityLogRepository repo,
            IMapper mapper,
            IValidator<ActivityLogPagedQueryDto> validator)
        {
            _repo = repo;
            _mapper = mapper;
            _validator = validator;
        }

        public IEnumerable<ActivityLogDto> GetAllDto()
        {
            var logs = _repo.GetLogs();
            return _mapper.Map<IEnumerable<ActivityLogDto>>(logs);
        }

        public PagedResultDto<ActivityLogDto> GetFiltered(ActivityLogPagedQueryDto query)
        {
            var vr = _validator.Validate(query);
            if (!vr.IsValid) throw new FluentValidation.ValidationException(vr.Errors);

            var q = _repo.GetLogs().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                q = q.Where(x => (x.Action ?? "").Contains(query.SearchTerm));

            if (query.FromDate.HasValue) q = q.Where(x => x.CreatedAt >= query.FromDate.Value);
            if (query.ToDate.HasValue) q = q.Where(x => x.CreatedAt <= query.ToDate.Value);

            var total = q.Count();

            if (query.PageNumber > 0 && query.PageSize > 0)
                q = q.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);

            var items = _mapper.Map<IEnumerable<ActivityLogDto>>(q.ToList());

            return new PagedResultDto<ActivityLogDto>
            {
                Data = items,
                TotalCount = total,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)query.PageSize)
            };
        }
    }
}
