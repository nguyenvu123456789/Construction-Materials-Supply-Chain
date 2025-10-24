using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Implementations
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ScmVlxdContext context) : base(context) { }

        public List<Vehicle> Search(string? q, bool? active, int? top, int? partnerId)
        {
            var s = _context.Vehicles.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                s = s.Where(x => x.Code.Contains(q) || x.PlateNumber.Contains(q) || (x.VehicleClass ?? "").Contains(q));
            if (active.HasValue)
                s = s.Where(x => x.Active == active.Value);
            if (partnerId.HasValue)
                s = s.Where(x => x.PartnerId == partnerId.Value);
            if (top.HasValue && top.Value > 0)
                s = s.Take(top.Value);
            return s.OrderBy(x => x.Code).ToList();
        }

        public List<Vehicle> GetByIds(IEnumerable<int> ids)
            => _context.Vehicles.Where(v => ids.Contains(v.VehicleId)).ToList();
    }
}
