using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        AuthResponseDto Register(RegisterRequestDto request);
        AuthResponseDto? Login(LoginRequestDto request);
        void Logout(int userId);
    }
}
