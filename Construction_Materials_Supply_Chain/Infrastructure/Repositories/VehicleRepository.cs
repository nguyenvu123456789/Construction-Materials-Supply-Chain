using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ScmVlxdContext context) : base(context) { }

        public List<Vehicle> Search(string? q, bool? active, int? top)
        {
            var s = _context.Vehicles.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                s = s.Where(x => x.Code.Contains(q) || x.PlateNumber.Contains(q) || (x.VehicleClass ?? "").Contains(q));
            if (active.HasValue) s = s.Where(x => x.Active == active.Value);
            if (top.HasValue && top.Value > 0) s = s.Take(top.Value);
            return s.OrderBy(x => x.Code).ToList();
        }
    }
}
