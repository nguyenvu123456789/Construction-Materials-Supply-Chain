using BusinessObjects;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public class UserRepository : IUserRepository
    {
        public List<User> GetUsers() => UserDAO.GetUsers();
        public List<User> SearchUsers(string keyword) => UserDAO.SearchUsers(keyword);
        public User GetUserById(int id) => UserDAO.FindUserById(id);
        public void SaveUser(User u) => UserDAO.SaveUser(u);
        public void UpdateUser(User u) => UserDAO.UpdateUser(u);
        public void DeleteUserById(int id) => UserDAO.DeleteUserById(id);
    }
}
