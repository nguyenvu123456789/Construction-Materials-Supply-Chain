using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Implementations
{
    public class ShippingLogService : IShippingLogService
    {
        private readonly IShippingLogRepository _repo;

        public ShippingLogService(IShippingLogRepository repo)
        {
            _repo = repo;
        }

        public List<ShippingLog> GetAll() => _repo.GetAll();

        public List<ShippingLog> SearchByStatus(string status)
        {
            var all = _repo.GetAll();
            if (string.IsNullOrWhiteSpace(status)) return all;
            return all.Where(s => (s.Status ?? "").Contains(status)).ToList();
        }
    }
}
