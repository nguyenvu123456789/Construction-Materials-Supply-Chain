namespace Repositories.Interface
{
    public interface IMaterialCheckRepository
    {
        List<MaterialCheck> GetAll();
        MaterialCheck GetById(int id);
        void Save(MaterialCheck mc);
        void Update(MaterialCheck mc);
        void Delete(int id);
    }
}
