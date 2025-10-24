using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Implementations
{
    public class DriverRepository : GenericRepository<Driver>, IDriverRepository
    {
        public DriverRepository(ScmVlxdContext context) : base(context) { }

        public List<Driver> Search(string? q, bool? active, int? top, int? partnerId)
        {
            var s = _context.Drivers.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                s = s.Where(x => x.FullName.Contains(q) || (x.Phone ?? "").Contains(q));
            if (active.HasValue)
                s = s.Where(x => x.Active == active.Value);
            if (partnerId.HasValue)
                s = s.Where(x => x.PartnerId == partnerId.Value);
            if (top.HasValue && top.Value > 0)
                s = s.Take(top.Value);
            return s.OrderBy(x => x.FullName).ToList();
        }

        public List<Driver> GetByIds(IEnumerable<int> ids)
            => _context.Drivers.Where(d => ids.Contains(d.DriverId)).ToList();
    }
}
