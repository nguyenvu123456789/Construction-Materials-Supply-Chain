using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouses;
        private readonly IUserRepository _userRepository;


        public WarehouseService(IWarehouseRepository warehouses, IUserRepository userRepository)
        {
            _warehouses = warehouses;
            _userRepository = userRepository;
        }

        public List<Warehouse> GetAll(int? managerId = null, int? partnerId = null)
        {
            // Lấy tất cả warehouse từ repository
            var warehouses = _warehouses.GetAll(); // trả về List<Warehouse>

            // Lọc theo managerId nếu có
            if (managerId.HasValue)
                warehouses = warehouses.Where(w => w.ManagerId == managerId.Value).ToList();

            // Lọc theo partnerId nếu có
            if (partnerId.HasValue)
                warehouses = warehouses.Where(w => w.Manager != null && w.Manager.PartnerId == partnerId.Value).ToList();

            return warehouses;
        }



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
        public List<Warehouse> GetByPartnerId(int partnerId)
        {
            return _warehouses.GetByPartnerId(partnerId);
        }

    }
}
