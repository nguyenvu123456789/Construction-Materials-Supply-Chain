namespace Application.DTOs.Common
{
    public interface ITenantContext
    {
        int? PartnerId { get; }
        IEnumerable<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}
