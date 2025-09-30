using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        List<User> SearchUsers(string keyword);
        User GetUserById(int id);
        void SaveUser(User u);
        void UpdateUser(User u);
        void DeleteUserById(int id);
    }
}
