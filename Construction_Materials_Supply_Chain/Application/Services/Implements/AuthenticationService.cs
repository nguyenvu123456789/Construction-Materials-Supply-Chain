using Application.Constants.Enums;
using Application.DTOs;
using Application.Interfaces;
using Application.Services.Auth;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Implements
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _users;
        private readonly IActivityLogRepository _activityLogs;
        private readonly IMapper _mapper;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IEmailChannel _emailChannel;
        private readonly IUserOtpRepository _userOtps;

        public AuthenticationService(
            IUserRepository users,
            IActivityLogRepository activityLogs,
            IMapper mapper,
            IValidator<LoginRequestDto> loginValidator,
            IJwtTokenGenerator jwt,
            IEmailChannel emailChannel,
            IUserOtpRepository userOtps
        )
        {
            _users = users;
            _activityLogs = activityLogs;
            _mapper = mapper;
            _loginValidator = loginValidator;
            _jwt = jwt;
            _emailChannel = emailChannel;
            _userOtps = userOtps;
        }

        public AuthResponseDto? Login(LoginRequestDto request)
        {
            var vr = _loginValidator.Validate(request);
            if (!vr.IsValid) throw new ValidationException(vr.Errors);

            var user = _users.QueryWithRolesAndPartner()
                             .FirstOrDefault(u => u.UserName == request.UserName);
            if (user is null) return null;
            if (!string.Equals(user.Status, "Active", StringComparison.OrdinalIgnoreCase)) return null;

            var hashB64 = HashBase64(request.Password);
            var hashHex = HashHex(request.Password);
            var ok = string.Equals(user.PasswordHash, hashB64, StringComparison.Ordinal)
                     || string.Equals(user.PasswordHash, hashHex, StringComparison.OrdinalIgnoreCase);
            if (!ok) return null;

            _activityLogs.LogAction(user.UserId, "User logged in", "User", user.UserId);

            var dto = _mapper.Map<AuthResponseDto>(user);
            dto.PartnerId = user.PartnerId;
            dto.PartnerName = user.Partner?.PartnerName;
            dto.PartnerType = user.Partner?.PartnerType?.TypeName;
            dto.MustChangePassword = user.MustChangePassword;

            var roles = _users.GetRoleNamesByUserId(user.UserId) ?? Enumerable.Empty<string>();
            dto.Roles = roles;
            dto.Token = _jwt.GenerateToken(user, roles);

            return dto;
        }

        public void Logout(int userId)
        {
            _activityLogs.LogAction(userId, "User logged out", "User", userId);
        }

        private static string HashBase64(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private static string HashHex(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private static string GenerateRandomPassword(int length = 12)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%^&*";
            var bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                var idx = bytes[i] % chars.Length;
                result.Append(chars[idx]);
            }
            return result.ToString();
        }

        public AuthResponseDto AdminCreateUser(AdminCreateUserRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException("Email is required");

            var email = request.Email.Trim();

            if (_users.ExistsByEmail(email))
                throw new InvalidOperationException("Email already exists");

            var userName = email;
            var rawPassword = GenerateRandomPassword();
            var passwordHash = HashBase64(rawPassword);

            var status = string.IsNullOrWhiteSpace(request.Status)
                ? "Active"
                : request.Status.Trim();

            var user = new User
            {
                UserName = userName,
                Email = email,
                PasswordHash = passwordHash,
                Status = status,
                CreatedAt = DateTime.UtcNow,
                MustChangePassword = true,
                FullName = string.IsNullOrWhiteSpace(request.FullName) ? null : request.FullName.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                PartnerId = request.PartnerId,
                AvatarBase64 = null,
                ZaloUserId = null
            };

            _users.Add(user);

            var subject = "Tài khoản SCM VLXD của bạn";
            var body = $"Tài khoản của bạn đã được tạo.\n" +
                       $"Tên đăng nhập: {userName}\n" +
                       $"Mật khẩu tạm: {rawPassword}\n\n" +
                       $"Vui lòng đăng nhập và đổi mật khẩu ngay lần đầu.";

            _emailChannel.SendAsync(user.PartnerId ?? 0, new[] { email }, subject, body);

            var dto = _mapper.Map<AuthResponseDto>(user);
            dto.MustChangePassword = true;
            dto.Roles = _users.GetRoleNamesByUserId(user.UserId) ?? Enumerable.Empty<string>();
            dto.Token = _jwt.GenerateToken(user, dto.Roles);

            return dto;
        }

        public void ChangePassword(ChangePasswordRequestDto request)
        {
            var user = _users.GetById(request.UserId);
            if (user == null) throw new InvalidOperationException("User not found");
            if (!string.Equals(user.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("User is not active");

            var currentHash = HashBase64(request.OldPassword);
            var currentHex = HashHex(request.OldPassword);
            var ok = string.Equals(user.PasswordHash, currentHash, StringComparison.Ordinal)
                     || string.Equals(user.PasswordHash, currentHex, StringComparison.OrdinalIgnoreCase);
            if (!ok) throw new InvalidOperationException("Old password is incorrect");

            user.PasswordHash = HashBase64(request.NewPassword);
            user.MustChangePassword = false;

            _users.Update(user);
            _activityLogs.LogAction(user.UserId, "User changed password", "User", user.UserId);
        }

        public async Task<List<AuthResponseDto>> BulkCreateUsersByEmailAsync(
            BulkCreateUsersByEmailRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var results = new List<AuthResponseDto>();

            if (request.Emails == null || request.Emails.Count == 0)
                return results;

            var distinctEmails = request.Emails
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var email in distinctEmails)
            {
                if (_users.ExistsByEmail(email))
                    continue;

                var rawPassword = GenerateRandomPassword();
                var passwordHash = HashBase64(rawPassword);

                var user = new User
                {
                    UserName = email,
                    Email = email,
                    PasswordHash = passwordHash,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    MustChangePassword = true
                };

                _users.Add(user);

                var subject = "Tài khoản SCM VLXD của bạn";
                var body = $"Tài khoản của bạn đã được tạo.\n" +
                           $"Tên đăng nhập: {email}\n" +
                           $"Mật khẩu tạm: {rawPassword}\n\n" +
                           $"Vui lòng đăng nhập và đổi mật khẩu ngay lần đầu.";

                await _emailChannel.SendAsync(user.PartnerId ?? 0, new[] { email }, subject, body, cancellationToken);

                var dto = _mapper.Map<AuthResponseDto>(user);
                dto.MustChangePassword = true;
                results.Add(dto);
            }

            return results;
        }

        private static string GenerateOtpCode(int length = 6)
        {
            const string digits = "0123456789";
            var bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(digits[bytes[i] % digits.Length]);
            }
            return sb.ToString();
        }
        public async Task RequestOtpAsync(OtpRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException("Email is required");
            if (string.IsNullOrWhiteSpace(request.Purpose))
                throw new ValidationException("Purpose is required");

            var email = request.Email.Trim();
            var user = _users.GetByEmail(email);
            if (user == null)
                throw new InvalidOperationException("User not found");

            _userOtps.InvalidateAll(user.UserId, request.Purpose);

            var code = GenerateOtpCode();
            var now = DateTime.UtcNow;

            var otp = new UserOtp
            {
                UserId = user.UserId,
                Code = code,
                Purpose = request.Purpose,
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(10),
                IsUsed = false
            };

            _userOtps.Add(otp);

            var subject = "Mã OTP xác thực";
            var body = $"Mã OTP của bạn là: {code}. Mã có hiệu lực trong 10 phút.";
            await _emailChannel.SendAsync(user.PartnerId ?? 0, new[] { email }, subject, body);
        }

        public Task<bool> VerifyOtpAsync(OtpVerifyDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException("Email is required");
            if (string.IsNullOrWhiteSpace(request.Purpose))
                throw new ValidationException("Purpose is required");
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ValidationException("Code is required");

            var email = request.Email.Trim();
            var user = _users.GetByEmail(email);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var otp = _userOtps.GetActive(user.UserId, request.Purpose, request.Code.Trim());
            if (otp == null)
                return Task.FromResult(false);

            otp.IsUsed = true;
            _userOtps.Update(otp);

            return Task.FromResult(true);
        }

        public async Task ForgotPasswordRequestAsync(ForgotPasswordRequestDto request)
        {
            var dto = new OtpRequestDto
            {
                Email = request.Email,
                Purpose = OtpPurposeEnum.ForgotPassword.ToString()
            };
            await RequestOtpAsync(dto);
        }

        public async Task ResetPasswordWithOtpAsync(ResetPasswordWithOtpDto request)
        {
            var email = request.Email.Trim();
            var user = _users.GetByEmail(email);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var otp = _userOtps.GetActive(user.UserId, OtpPurposeEnum.ForgotPassword.ToString(), request.OtpCode.Trim());
            if (otp == null)
                throw new InvalidOperationException("Invalid or expired OTP");

            otp.IsUsed = true;
            _userOtps.Update(otp);

            user.PasswordHash = HashBase64(request.NewPassword);
            user.MustChangePassword = false;
            _users.Update(user);

            _activityLogs.LogAction(user.UserId, "User reset password via OTP", "User", user.UserId);
        }
    }
}
