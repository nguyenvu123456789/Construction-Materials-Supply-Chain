using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class MaterialCheckDAO : BaseDAO
    {
        public MaterialCheckDAO(ScmVlxdContext context) : base(context) { }

        public List<MaterialCheck> GetAll() => Context.MaterialChecks
            .Include(mc => mc.Material)
            .Include(mc => mc.User)
            .ToList();

        public MaterialCheck GetById(int id) => Context.MaterialChecks
            .Include(mc => mc.Material)
            .Include(mc => mc.User)
            .SingleOrDefault(mc => mc.CheckId == id);

        public void Save(MaterialCheck mc)
        {
            Context.MaterialChecks.Add(mc);
            Context.SaveChanges();
        }

        public void Update(MaterialCheck mc)
        {
            Context.Entry(mc).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public void Delete(int id)
        {
            var mc = Context.MaterialChecks.Find(id);
            if (mc != null)
            {
                Context.MaterialChecks.Remove(mc);
                Context.SaveChanges();
            }
        }
    }
}
