using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _dao;

        public UserRepository(UserDAO dao)
        {
            _dao = dao;
        }

        public List<User> GetUsers() => _dao.GetUsers();
        public List<User> SearchUsers(string keyword) => _dao.SearchUsers(keyword);
        public User GetUserById(int id) => _dao.FindUserById(id);
        public void SaveUser(User u) => _dao.SaveUser(u);
        public void UpdateUser(User u) => _dao.UpdateUser(u);
        public void DeleteUserById(int id) => _dao.DeleteUserById(id);
    }
}
