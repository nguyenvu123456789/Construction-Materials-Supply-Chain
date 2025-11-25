using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IUserOtpRepository : IGenericRepository<UserOtp>
    {
        UserOtp? GetActive(int userId, string purpose, string code);
        void InvalidateAll(int userId, string purpose);
    }
}
