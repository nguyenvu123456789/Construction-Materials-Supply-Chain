using Application.Common.Pagination;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IActivityLogService
    {
        IEnumerable<ActivityLogDto> GetAllDto();
        PagedResultDto<ActivityLogDto> GetFiltered(ActivityLogPagedQueryDto query);
    }
}