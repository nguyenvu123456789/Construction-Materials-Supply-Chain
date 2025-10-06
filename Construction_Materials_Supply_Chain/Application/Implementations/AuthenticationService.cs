using Application.Interfaces;
using Domain;
using Infrastructure.Persistence;
using Infrastructure.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ScmVlxdContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IActivityLogRepository _activityLogRepository;

        public AuthenticationService(ScmVlxdContext context, IUserRepository userRepository, IActivityLogRepository activityLogRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _activityLogRepository = activityLogRepository;
        }

        public User Register(string userName, string password, string email)
        {
            if (_context.Users.Any(u => u.UserName == userName))
                throw new InvalidOperationException("Username already exists");

            var user = new User
            {
                UserName = userName,
                PasswordHash = Hash(password),
                Email = email,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public User? Login(string userName, string password)
        {
            var user = _userRepository.GetByUsername(userName);
            if (user == null) return null;

            var hash = Hash(password);
            if (!string.Equals(user.PasswordHash, hash, StringComparison.Ordinal))
                return null;

            _activityLogRepository.LogAction(user.UserId, "User logged in", "User", user.UserId);
            return user;
        }

        public void Logout(int userId)
        {
            _activityLogRepository.LogAction(userId, "User logged out", "User", userId);
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
