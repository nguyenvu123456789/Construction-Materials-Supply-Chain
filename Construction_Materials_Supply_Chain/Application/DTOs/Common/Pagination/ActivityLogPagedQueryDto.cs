namespace Application.Common.Pagination
{
    public class ActivityLogPagedQueryDto : PagedQueryDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}