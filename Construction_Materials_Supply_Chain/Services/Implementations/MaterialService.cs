using System.Collections.Generic;
using BusinessObjects;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _repo;

        public MaterialService(IMaterialRepository repo)
        {
            _repo = repo;
        }

        public List<Material> GetAll() => _repo.GetAll();

        public Material? GetById(int id) => _repo.GetById(id);

        public void Create(Material material) => _repo.Add(material);

        public void Update(Material material) => _repo.Update(material);

        public void Delete(int id)
        {
            var m = _repo.GetById(id);
            if (m != null)
                _repo.Delete(m);
        }
    }
}
