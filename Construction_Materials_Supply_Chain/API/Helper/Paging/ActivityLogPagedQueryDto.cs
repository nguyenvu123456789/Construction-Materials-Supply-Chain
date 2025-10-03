namespace API.Helper.Paging
{
    public class ActivityLogPagedQueryDto : PagedQueryDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}