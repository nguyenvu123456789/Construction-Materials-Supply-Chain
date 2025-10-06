using Domain;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        User Register(string userName, string password, string email);

        User? Login(string userName, string password);

        void Logout(int userId);
    }
}
