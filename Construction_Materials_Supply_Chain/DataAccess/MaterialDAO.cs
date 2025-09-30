using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class MaterialDAO
    {
        private readonly ScmVlxdContext _context;

        public MaterialDAO(ScmVlxdContext context)
        {
            _context = context;
        }

        public List<Material> GetMaterials()
        {
            return _context.Materials.Include(m => m.Category).ToList();
        }

        public Material? GetMaterialById(int id)
        {
            return _context.Materials.Include(m => m.Category)
                                     .FirstOrDefault(m => m.MaterialId == id);
        }

        public void AddMaterial(Material material)
        {
            _context.Materials.Add(material);
            _context.SaveChanges();
        }

        public void UpdateMaterial(Material material)
        {
            _context.Materials.Update(material);
            _context.SaveChanges();
        }

        public void DeleteMaterial(int id)
        {
            var material = _context.Materials.Find(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
                _context.SaveChanges();
            }
        }
    }
}
