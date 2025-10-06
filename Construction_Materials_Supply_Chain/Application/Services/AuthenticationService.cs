using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Models;
using FluentValidation;
using System.Security.Cryptography;
using System.Text;

namespace Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _users;
        private readonly IActivityLogRepository _activityLogs;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterRequestDto> _registerValidator;
        private readonly IValidator<LoginRequestDto> _loginValidator;

        public AuthenticationService(
            IUserRepository users,
            IActivityLogRepository activityLogs,
            IMapper mapper,
            IValidator<RegisterRequestDto> registerValidator,
            IValidator<LoginRequestDto> loginValidator)
        {
            _users = users;
            _activityLogs = activityLogs;
            _mapper = mapper;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        public AuthResponseDto Register(RegisterRequestDto request)
        {
            var vr = _registerValidator.Validate(request);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            if (_users.ExistsByUsername(request.UserName))
                throw new InvalidOperationException("Username already exists");

            var user = new User
            {
                UserName = request.UserName,
                PasswordHash = Hash(request.Password),
                Email = request.Email,
                CreatedAt = DateTime.UtcNow
            };

            _users.Add(user);
            return _mapper.Map<AuthResponseDto>(user);
        }

        public AuthResponseDto? Login(LoginRequestDto request)
        {
            var vr = _loginValidator.Validate(request);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            var user = _users.GetByUsername(request.UserName);
            if (user is null) return null;

            var hash = Hash(request.Password);
            if (!string.Equals(user.PasswordHash, hash, StringComparison.Ordinal)) return null;

            _activityLogs.LogAction(user.UserId, "User logged in", "User", user.UserId);
            return _mapper.Map<AuthResponseDto>(user);
        }

        public void Logout(int userId)
        {
            _activityLogs.LogAction(userId, "User logged out", "User", userId);
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
