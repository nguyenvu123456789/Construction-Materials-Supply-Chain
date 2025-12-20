using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        AuthResponseDto? Login(LoginRequestDto request);
        void Logout(int userId, string token);
        AuthResponseDto AdminCreateUser(AdminCreateUserRequestDto request);
        void ChangePassword(ChangePasswordRequestDto request);
        Task<List<AuthResponseDto>> BulkCreateUsersByEmailAsync(BulkCreateUsersByEmailRequestDto request, CancellationToken cancellationToken = default);
        Task RequestOtpAsync(OtpRequestDto request);
        Task<bool> VerifyOtpAsync(OtpVerifyDto request);
        Task ForgotPasswordRequestAsync(ForgotPasswordRequestDto request);
        Task ResetPasswordWithOtpAsync(ResetPasswordWithOtpDto request); 
    }
}
