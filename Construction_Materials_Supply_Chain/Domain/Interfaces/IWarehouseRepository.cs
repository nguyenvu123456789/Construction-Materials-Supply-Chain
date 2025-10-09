using Domain.Models;

namespace Domain.Interface
{
    public interface IWarehouseRepository
    {
        List<Warehouse> GetAll();
        Warehouse? GetById(int id);
        void Add(Warehouse warehouse);
        void Update(Warehouse warehouse);
        void Delete(Warehouse warehouse);
    }
}
