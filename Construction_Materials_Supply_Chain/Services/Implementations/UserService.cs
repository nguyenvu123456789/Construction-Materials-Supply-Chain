using Application.Interfaces;
using Domain;
using Infrastructure.Interface;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;

        public UserService(IUserRepository users)
        {
            _users = users;
        }

        public List<User> GetAll() => _users.GetAll();

        public User? GetById(int id) => _users.GetById(id);

        public void Create(User user) => _users.Add(user);

        public void Update(User user) => _users.Update(user);

        public void Delete(int id)
        {
            var u = _users.GetById(id);
            if (u != null)
                _users.Delete(u);
        }

        public List<User> GetUsersFiltered(string? searchTerm, List<string>? roles, int pageNumber, int pageSize, out int totalCount)
        {
            var query = _users.GetAll().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(u =>
                    (u.UserName ?? "").Contains(searchTerm) ||
                    (u.Email ?? "").Contains(searchTerm));

            totalCount = query.Count();
            if (pageNumber > 0 && pageSize > 0)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }
    }
}
