using System.Collections.Generic;
using BusinessObjects;

namespace Services.Interfaces
{
    public interface IUserService
    {
        List<User> GetAll();
        User? GetById(int id);
        User? GetByUsername(string username);
        void Create(User user);
        void Update(User user);
        void Delete(int id);

        List<User> GetUsersFiltered(string? searchTerm, List<string>? roles, int pageNumber, int pageSize, out int totalCount);
    }
}
