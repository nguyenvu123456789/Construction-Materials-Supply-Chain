using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class ShippingLogService : IShippingLogService
    {
        private readonly IShippingLogRepository _repo;

        public ShippingLogService(IShippingLogRepository repo)
        {
            _repo = repo;
        }

        public List<ShippingLog> GetAll() => _repo.GetAll();
    }
}
