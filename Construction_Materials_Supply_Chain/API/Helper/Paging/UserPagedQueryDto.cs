namespace API.Helper.Paging
{
    public class UserPagedQueryDto : PagedQueryDto
    {
        public List<string>? Roles { get; set; }
    }
}
