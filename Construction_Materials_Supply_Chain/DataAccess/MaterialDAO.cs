using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class MaterialDAO : BaseDAO
    {
        public MaterialDAO(ScmVlxdContext context) : base(context) { }

        public List<Material> GetMaterials()
        {
            return Context.Materials
                           .Include(m => m.Category)
                           .ToList();
        }

        public Material? GetMaterialById(int id)
        {
            return Context.Materials
                           .Include(m => m.Category)
                           .FirstOrDefault(m => m.MaterialId == id);
        }

        public void AddMaterial(Material material)
        {
            Context.Materials.Add(material);
            Context.SaveChanges();
        }

        public void UpdateMaterial(Material material)
        {
            Context.Materials.Update(material);
            Context.SaveChanges();
        }

        public void DeleteMaterial(int id)
        {
            var material = Context.Materials.Find(id);
            if (material != null)
            {
                Context.Materials.Remove(material);
                Context.SaveChanges();
            }
        }
    }
}
