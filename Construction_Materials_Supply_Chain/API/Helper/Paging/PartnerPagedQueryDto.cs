namespace API.Helper.Paging
{
    public class PartnerPagedQueryDto : PagedQueryDto
    {
        public List<string>? PartnerTypes { get; set; }
    }
}