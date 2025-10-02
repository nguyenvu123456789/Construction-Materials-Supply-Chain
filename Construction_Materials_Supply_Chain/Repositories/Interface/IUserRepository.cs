using BusinessObjects;

namespace Repositories.Interface
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        User GetUserById(int id);
        void SaveUser(User u);
        void UpdateUser(User u);
        void DeleteUserById(int id);

        List<User> GetUsersPaged(string? keyword, int pageNumber, int pageSize);
        int GetTotalUsersCount(string? keyword);
    }
}
