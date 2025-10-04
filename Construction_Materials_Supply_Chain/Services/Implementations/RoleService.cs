using System.Collections.Generic;
using Repositories.Interface;
using BusinessObjects;
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