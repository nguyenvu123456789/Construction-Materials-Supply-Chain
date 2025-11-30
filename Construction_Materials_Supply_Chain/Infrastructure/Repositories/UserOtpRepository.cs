using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class UserOtpRepository : GenericRepository<UserOtp>, IUserOtpRepository
    {
        public UserOtpRepository(ScmVlxdContext context) : base(context)
        {
        }

        public UserOtp? GetActive(int userId, string purpose, string code)
        {
            var now = DateTime.Now;
            return _context.UserOtps
                .FirstOrDefault(x =>
                    x.UserId == userId &&
                    x.Purpose == purpose &&
                    x.Code == code &&
                    !x.IsUsed &&
                    x.ExpiresAt >= now);
        }

        public void InvalidateAll(int userId, string purpose)
        {
            var list = _context.UserOtps
                .Where(x => x.UserId == userId && x.Purpose == purpose && !x.IsUsed)
                .ToList();
            if (list.Count == 0) return;
            foreach (var item in list)
            {
                item.IsUsed = true;
            }
            _context.SaveChanges();
        }
    }
}
