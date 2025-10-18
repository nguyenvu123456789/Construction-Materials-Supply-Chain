using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IWarehouseService
    {
        List<Warehouse> GetAll();
        Warehouse? GetById(int id);
        Warehouse Create(WarehouseCreateDto dto);
        Warehouse Update(int id, WarehouseUpdateDto dto);
        bool Delete(int id);
    }
}
