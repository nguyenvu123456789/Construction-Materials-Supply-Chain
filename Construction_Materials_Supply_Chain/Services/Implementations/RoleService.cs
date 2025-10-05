using BusinessObjects;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;
        public RoleService(IRoleRepository repo) { _repo = repo; }
        public List<Role> GetAll() => _repo.GetAll();
    }
}