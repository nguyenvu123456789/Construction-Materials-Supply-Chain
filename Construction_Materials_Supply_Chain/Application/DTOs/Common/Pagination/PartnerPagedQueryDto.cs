namespace Application.Common.Pagination
{
    public class PartnerPagedQueryDto : PagedQueryDto
    {
        public List<string>? PartnerTypes { get; set; }
    }
}