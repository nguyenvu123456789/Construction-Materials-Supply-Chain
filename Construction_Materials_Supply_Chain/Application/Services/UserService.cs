using Application.Interfaces;
using Domain.Interface;
using Domain.Models;
using System.Globalization;

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

        public List<User> GetAllWithRoles()
        => _users.QueryWithRoles().ToList();

        public User? GetByIdWithRoles(int id)
            => _users.QueryWithRoles().FirstOrDefault(u => u.UserId == id);

        public List<User> GetUsersFiltered(string? searchTerm, List<string>? roles,
        int pageNumber, int pageSize, out int totalCount)
        {
            var query = _users.QueryWithRoles();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(u =>
                    (u.UserName ?? "").Contains(term) ||
                    (u.Email ?? "").Contains(term) ||
                    (u.FullName ?? "").Contains(term));
            }

            if (roles != null && roles.Count > 0)
            {
                var set = roles
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Select(r => r.Trim().ToLower(CultureInfo.InvariantCulture))
                    .ToHashSet();

                query = query.Where(u => u.UserRoles.Any(ur =>
                    set.Contains(ur.Role.RoleName.ToLower())));
            }

            totalCount = query.Count();

            if (pageNumber > 0 && pageSize > 0)
            {
                query = query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
            }

            return query.ToList();
        }
    }
}
