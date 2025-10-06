namespace Application.Common.Pagination
{
    public class UserPagedQueryDto : PagedQueryDto
    {
        public List<string>? Roles { get; set; }
    }
}
