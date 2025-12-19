using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Implementations
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ScmVlxdContext context) : base(context) { }

        public List<Vehicle> Search(string? q, bool? active, int? top, int? partnerId)
        {
            var s = _context.Vehicles.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var pat = $"%{q}%";
                s = s.Where(x =>
                    EF.Functions.Like(x.Code, pat) ||
                    EF.Functions.Like(x.PlateNumber, pat) ||
                    EF.Functions.Like(x.VehicleClass ?? string.Empty, pat));
            }
            if (active.HasValue) s = s.Where(x => x.Active == active.Value);
            if (partnerId.HasValue) s = s.Where(x => x.PartnerId == partnerId.Value);
            if (top.HasValue && top.Value > 0) s = s.Take(top.Value);
            return s.OrderBy(x => x.Code).ToList();
        }

        public List<Vehicle> GetByIds(IEnumerable<int> ids)
        {
            var set = ids?.Distinct().ToList();
            if (set == null || set.Count == 0) return new List<Vehicle>();
            return _context.Vehicles.Where(v => set.Contains(v.VehicleId)).ToList();
        }

        public bool CheckExists(Expression<Func<Vehicle, bool>> predicate)
        {
            return _context.Vehicles.Any(predicate);
        }
    }
}
