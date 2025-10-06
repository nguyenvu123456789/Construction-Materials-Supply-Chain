using Application.Interfaces;
using Domain.Models;
using Domain.Interface;

namespace Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;
        public RoleService(IRoleRepository repo) { _repo = repo; }
        public List<Role> GetAll() => _repo.GetAll();
    }
}