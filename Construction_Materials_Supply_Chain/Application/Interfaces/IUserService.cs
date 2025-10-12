using Domain.Models;

namespace Application.Interfaces
{
    public interface IUserService
    {
        List<User> GetAll();
        User? GetById(int id);
        void Create(User user);
        void Update(User user);
        void Delete(int id);

        List<User> GetAllWithRoles();
        User? GetByIdWithRoles(int id);

        List<User> GetUsersFiltered(string? searchTerm, List<string>? roles, int pageNumber, int pageSize, out int totalCount);
    }
}
