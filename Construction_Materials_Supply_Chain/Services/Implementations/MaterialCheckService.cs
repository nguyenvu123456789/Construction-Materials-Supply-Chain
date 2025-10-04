using System.Collections.Generic;
using BusinessObjects;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Implementations
{
    public class MaterialCheckService : IMaterialCheckService
    {
        private readonly IMaterialCheckRepository _repo;

        public MaterialCheckService(IMaterialCheckRepository repo)
        {
            _repo = repo;
        }

        public List<MaterialCheck> GetAll() => _repo.GetAll();

        public MaterialCheck? GetById(int id) => _repo.GetById(id);

        public void Create(MaterialCheck check) => _repo.Add(check);

        public void Update(MaterialCheck check) => _repo.Update(check);

        public void Delete(int id)
        {
            var entity = _repo.GetById(id);
            if (entity != null)
                _repo.Delete(entity);
        }
    }
}
