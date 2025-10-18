using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class DriverRepository : GenericRepository<Driver>, IDriverRepository
    {
        public DriverRepository(ScmVlxdContext context) : base(context) { }

        public List<Driver> Search(string? q, bool? active, int? top)
        {
            var s = _context.Drivers.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q)) s = s.Where(x => x.FullName.Contains(q) || (x.Phone ?? "").Contains(q));
            if (active.HasValue) s = s.Where(x => x.Active == active.Value);
            if (top.HasValue && top.Value > 0) s = s.Take(top.Value);
            return s.OrderBy(x => x.FullName).ToList();
        }
    }
}
