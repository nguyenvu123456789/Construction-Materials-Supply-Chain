using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IWarehouseService
    {
        List<Warehouse> GetAll(int? managerId = null, int? partnerId = null);
        Warehouse? GetById(int id);
        List<Warehouse> GetByPartnerId(int partnerId);
        Warehouse Create(WarehouseCreateDto dto);
        Warehouse Update(int id, WarehouseUpdateDto dto);
        bool Delete(int id);
    }
}
