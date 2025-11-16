namespace Application.Services.Interfaces
{
    public interface IEmailChannel
    {
        Task SendAsync(
            int partnerId,
            IEnumerable<string> emailAddresses,
            string subject,
            string content,
            CancellationToken cancellationToken = default);
    }
}
