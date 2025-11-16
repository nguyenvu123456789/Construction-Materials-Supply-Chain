using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        AuthResponseDto? Login(LoginRequestDto request);
        void Logout(int userId);

        AuthResponseDto AdminCreateUser(AdminCreateUserRequestDto request);
        void ChangePassword(ChangePasswordRequestDto request);
        Task<List<AuthResponseDto>> BulkCreateUsersByEmailAsync(BulkCreateUsersByEmailRequestDto request, CancellationToken cancellationToken = default);
    }
}
