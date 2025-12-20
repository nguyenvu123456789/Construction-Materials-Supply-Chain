using Application.Constants.Enums;
using Application.Constants.Messages;
using Application.DTOs;
using Application.Interfaces;
using Application.Services.Auth;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Implements
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepo;
        private readonly IActivityLogRepository _logRepo;
        private readonly IUserOtpRepository _otpRepo;
        private readonly IMapper _mapper;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly IEmailChannel _emailChannel;
        private readonly ITokenBlacklistService _blacklistService;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IValidator<AdminCreateUserRequestDto> _adminCreateValidator;
        private readonly IValidator<ChangePasswordRequestDto> _changePasswordValidator;
        private readonly IValidator<OtpRequestDto> _otpRequestValidator;
        private readonly IValidator<OtpVerifyDto> _otpVerifyValidator;
        private readonly IValidator<ResetPasswordWithOtpDto> _resetPasswordValidator;

        public AuthenticationService(
            IUserRepository userRepo,
            IActivityLogRepository logRepo,
            IUserOtpRepository otpRepo,
            IMapper mapper,
            IJwtTokenGenerator jwtGenerator,
            IEmailChannel emailChannel,
            ITokenBlacklistService blacklistService,
            IValidator<LoginRequestDto> loginValidator,
            IValidator<AdminCreateUserRequestDto> adminCreateValidator,
            IValidator<ChangePasswordRequestDto> changePasswordValidator,
            IValidator<OtpRequestDto> otpRequestValidator,
            IValidator<OtpVerifyDto> otpVerifyValidator,
            IValidator<ResetPasswordWithOtpDto> resetPasswordValidator
        )
        {
            _userRepo = userRepo;
            _logRepo = logRepo;
            _otpRepo = otpRepo;
            _mapper = mapper;
            _jwtGenerator = jwtGenerator;
            _emailChannel = emailChannel;
            _blacklistService = blacklistService;
            _loginValidator = loginValidator;
            _adminCreateValidator = adminCreateValidator;
            _changePasswordValidator = changePasswordValidator;
            _otpRequestValidator = otpRequestValidator;
            _otpVerifyValidator = otpVerifyValidator;
            _resetPasswordValidator = resetPasswordValidator;
        }

        public AuthResponseDto? Login(LoginRequestDto request)
        {
            var validation = _loginValidator.Validate(request);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var user = _userRepo.QueryWithRolesAndPartner()
                                .FirstOrDefault(u => u.UserName == request.UserName);

            if (user == null) return null;

            if (!string.Equals(user.Status, "Active", StringComparison.OrdinalIgnoreCase))
                return null;

            if (!VerifyPassword(request.Password, user.PasswordHash))
                return null;

            _logRepo.LogAction(user.UserId, "User logged in", "User", user.UserId);

            return CreateAuthResponse(user);
        }

        public void Logout(int userId, string token)
        {
            _logRepo.LogAction(userId, "User logged out", "User", userId);

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                    var exp = jwtToken.ValidTo;

                    if (!string.IsNullOrEmpty(jti))
                    {
                        _blacklistService.BlacklistToken(jti, exp);
                    }
                }
                catch
                {
                }
            }
        }

        public AuthResponseDto AdminCreateUser(AdminCreateUserRequestDto request)
        {
            var validation = _adminCreateValidator.Validate(request);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var email = request.Email.Trim();

            if (_userRepo.ExistsByEmail(email))
                throw new InvalidOperationException(string.Format(AuthMessages.EMAIL_EXISTED, email));

            var rawPassword = GenerateRandomString(12);
            var passwordHash = HashBase64(rawPassword);

            var user = new User
            {
                UserName = email,
                Email = email,
                PasswordHash = passwordHash,
                Status = string.IsNullOrWhiteSpace(request.Status) ? "Active" : request.Status.Trim(),
                CreatedAt = DateTime.Now,
                MustChangePassword = true,
                FullName = request.FullName?.Trim(),
                Phone = request.Phone?.Trim(),
                PartnerId = request.PartnerId
            };

            _userRepo.Add(user);

            var body = $"Tài khoản của bạn đã được tạo.\n" +
                       $"Tên đăng nhập: {email}\n" +
                       $"Mật khẩu tạm: {rawPassword}\n\n" +
                       $"Vui lòng đăng nhập và đổi mật khẩu ngay lần đầu.";

            _emailChannel.SendAsync(user.PartnerId ?? 0, new[] { email }, AuthMessages.EMAIL_SUBJECT_NEW_ACCOUNT, body);

            return CreateAuthResponse(user, mustChangePass: true);
        }

        public async Task<List<AuthResponseDto>> BulkCreateUsersByEmailAsync(
            BulkCreateUsersByEmailRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var results = new List<AuthResponseDto>();
            if (request.Emails == null || !request.Emails.Any()) return results;

            var distinctEmails = request.Emails
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var email in distinctEmails)
            {
                if (_userRepo.ExistsByEmail(email)) continue;

                var rawPassword = GenerateRandomString(12);
                var user = new User
                {
                    UserName = email,
                    Email = email,
                    PasswordHash = HashBase64(rawPassword),
                    Status = "Active",
                    CreatedAt = DateTime.Now,
                    MustChangePassword = true
                };

                _userRepo.Add(user);

                var body = $"Tài khoản của bạn đã được tạo.\n" +
                           $"Tên đăng nhập: {email}\n" +
                           $"Mật khẩu tạm: {rawPassword}\n\n" +
                           $"Vui lòng đăng nhập và đổi mật khẩu ngay lần đầu.";

                await _emailChannel.SendAsync(user.PartnerId ?? 0, new[] { email }, AuthMessages.EMAIL_SUBJECT_NEW_ACCOUNT, body, cancellationToken);

                var dto = _mapper.Map<AuthResponseDto>(user);
                dto.MustChangePassword = true;
                results.Add(dto);
            }

            return results;
        }

        public void ChangePassword(ChangePasswordRequestDto request)
        {
            var validation = _changePasswordValidator.Validate(request);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var user = _userRepo.GetById(request.UserId);
            if (user == null)
                throw new KeyNotFoundException(AuthMessages.USER_NOT_FOUND);

            if (!string.Equals(user.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(AuthMessages.USER_INACTIVE);

            if (!VerifyPassword(request.OldPassword, user.PasswordHash))
                throw new InvalidOperationException(AuthMessages.OLD_PASSWORD_WRONG);

            user.PasswordHash = HashBase64(request.NewPassword);
            user.MustChangePassword = false;

            _userRepo.Update(user);
            _logRepo.LogAction(user.UserId, "User changed password", "User", user.UserId);
        }

        public async Task RequestOtpAsync(OtpRequestDto request)
        {
            var validation = _otpRequestValidator.Validate(request);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var email = request.Email.Trim();
            var user = _userRepo.GetByEmail(email);
            if (user == null)
                throw new KeyNotFoundException(AuthMessages.USER_NOT_FOUND);

            _otpRepo.InvalidateAll(user.UserId, request.Purpose);

            var code = GenerateRandomString(6, true);
            var otp = new UserOtp
            {
                UserId = user.UserId,
                Code = code,
                Purpose = request.Purpose,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(10),
                IsUsed = false
            };

            _otpRepo.Add(otp);

            var body = $"Mã OTP của bạn là: {code}. Mã có hiệu lực trong 10 phút.";
            await _emailChannel.SendAsync(user.PartnerId ?? 0, new[] { email }, AuthMessages.EMAIL_SUBJECT_OTP, body);
        }

        public Task<bool> VerifyOtpAsync(OtpVerifyDto request)
        {
            var validation = _otpVerifyValidator.Validate(request);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var user = _userRepo.GetByEmail(request.Email.Trim());
            if (user == null)
                throw new KeyNotFoundException(AuthMessages.USER_NOT_FOUND);

            var otp = _otpRepo.GetActive(user.UserId, request.Purpose, request.Code.Trim());
            if (otp == null) return Task.FromResult(false);

            otp.IsUsed = true;
            _otpRepo.Update(otp);

            return Task.FromResult(true);
        }

        public async Task ForgotPasswordRequestAsync(ForgotPasswordRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentNullException(nameof(request.Email));

            await RequestOtpAsync(new OtpRequestDto
            {
                Email = request.Email,
                Purpose = OtpPurposeEnum.ForgotPassword.ToString()
            });
        }

        public async Task ResetPasswordWithOtpAsync(ResetPasswordWithOtpDto request)
        {
            var validation = _resetPasswordValidator.Validate(request);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var user = _userRepo.GetByEmail(request.Email.Trim());
            if (user == null)
                throw new KeyNotFoundException(AuthMessages.USER_NOT_FOUND);

            var otp = _otpRepo.GetActive(user.UserId, OtpPurposeEnum.ForgotPassword.ToString(), request.OtpCode.Trim());
            if (otp == null)
                throw new InvalidOperationException(AuthMessages.OTP_INVALID_OR_EXPIRED);

            otp.IsUsed = true;
            _otpRepo.Update(otp);

            user.PasswordHash = HashBase64(request.NewPassword);
            user.MustChangePassword = false;
            _userRepo.Update(user);

            _logRepo.LogAction(user.UserId, "User reset password via OTP", "User", user.UserId);
            await Task.CompletedTask;
        }

        private AuthResponseDto CreateAuthResponse(User user, bool? mustChangePass = null)
        {
            var dto = _mapper.Map<AuthResponseDto>(user);
            dto.PartnerId = user.PartnerId;
            dto.PartnerName = user.Partner?.PartnerName;
            dto.PartnerType = user.Partner?.PartnerType?.TypeName;
            dto.MustChangePassword = mustChangePass ?? user.MustChangePassword;

            var roles = _userRepo.GetRoleNamesByUserId(user.UserId) ?? Enumerable.Empty<string>();
            dto.Roles = roles;
            dto.Token = _jwtGenerator.GenerateToken(user, roles);

            return dto;
        }

        private static bool VerifyPassword(string inputPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash)) return false;

            var hashB64 = HashBase64(inputPassword);
            var hashHex = HashHex(inputPassword);

            return string.Equals(storedHash, hashB64, StringComparison.Ordinal)
                || string.Equals(storedHash, hashHex, StringComparison.OrdinalIgnoreCase);
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
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static string GenerateRandomString(int length, bool digitsOnly = false)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%^&*";
            const string digits = "0123456789";

            var charSet = digitsOnly ? digits : chars;
            var bytes = new byte[length];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(charSet[bytes[i] % charSet.Length]);
            }
            return result.ToString();
        }
    }
}