using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouses;

        public WarehouseService(IWarehouseRepository warehouses)
        {
            _warehouses = warehouses;
        }

        public List<Warehouse> GetAll() => _warehouses.GetAll();

        public Warehouse? GetById(int id) => _warehouses.GetById(id);

        public Warehouse Create(WarehouseCreateDto dto)
        {
            var warehouse = new Warehouse
            {
                WarehouseName = dto.WarehouseName,
                Location = dto.Location,
                ManagerId = dto.ManagerId
            };
            _warehouses.Add(warehouse);
            return warehouse;
        }

        public Warehouse Update(int id, WarehouseUpdateDto dto)
        {
            var warehouse = _warehouses.GetById(id);
            if (warehouse == null)
                throw new Exception("Warehouse not found.");

            warehouse.WarehouseName = dto.WarehouseName;
            warehouse.Location = dto.Location;
            warehouse.ManagerId = dto.ManagerId;

            _warehouses.Update(warehouse);
            return warehouse;
        }

        public bool Delete(int id)
        {
            var warehouse = _warehouses.GetById(id);
            if (warehouse == null) return false;

            _warehouses.Delete(warehouse);
            return true;
        }
    }
}
