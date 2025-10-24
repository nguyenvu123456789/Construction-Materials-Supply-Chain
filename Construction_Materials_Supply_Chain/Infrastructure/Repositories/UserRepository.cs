using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ScmVlxdContext context) : base(context) { }

        public List<User> GetAllNotDeleted() =>
            _dbSet.AsNoTracking().Where(u => u.Status != "Deleted").ToList();

        public User? GetByIdNotDeleted(int id) =>
            _dbSet.FirstOrDefault(u => u.UserId == id && u.Status != "Deleted");

        public User? GetByUsername(string username) =>
            _dbSet.FirstOrDefault(u => u.UserName == username && u.Status != "Deleted");

        public bool ExistsByUsername(string username) =>
            _dbSet.Any(u => u.UserName == username && u.Status != "Deleted");

        public IQueryable<User> QueryWithRoles() =>
            _dbSet.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                  .Where(u => u.Status != "Deleted");

        public IQueryable<User> QueryWithRolesIncludeDeleted() =>
            _dbSet.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);

        public void SoftDelete(User entity)
        {
            entity.Status = "Deleted";
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public IEnumerable<string> GetRoleNamesByUserId(int userId) =>
            _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.RoleName!.Trim())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .ToList();

        public void AssignRoles(int userId, IEnumerable<int> roleIds)
        {
            var existing = _context.UserRoles.Where(ur => ur.UserId == userId).ToList();
            _context.UserRoles.RemoveRange(existing);
            foreach (var rid in roleIds.Distinct())
            {
                _context.UserRoles.Add(new UserRole { UserId = userId, RoleId = rid });
            }
            _context.SaveChanges();
        }

        public IQueryable<User> QueryWithRolesAndPartner()
            => _dbSet
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Partner).ThenInclude(p => p.PartnerType);

        public User? GetByIdWithPartner(int id)
        {
            return _dbSet
                .Include(u => u.Partner)
                .ThenInclude(p => p.PartnerType) 
                .FirstOrDefault(u => u.UserId == id && u.Status != "Deleted");
        }
    }
}
