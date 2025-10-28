using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class PorterRepository : GenericRepository<Porter>, IPorterRepository
    {
        public PorterRepository(ScmVlxdContext context) : base(context) { }

        public List<Porter> Search(string? q, bool? active, int? top, int? partnerId)
        {
            var s = _context.Porters.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var pat = $"%{q}%";
                s = s.Where(x =>
                    EF.Functions.Like(x.FullName, pat) ||
                    EF.Functions.Like(x.Phone ?? string.Empty, pat));
            }
            if (active.HasValue) s = s.Where(x => x.Active == active.Value);
            if (partnerId.HasValue) s = s.Where(x => x.PartnerId == partnerId.Value);
            if (top.HasValue && top.Value > 0) s = s.Take(top.Value);
            return s.OrderBy(x => x.FullName).ToList();
        }

        public List<Porter> GetByIds(IEnumerable<int> ids)
        {
            var set = ids?.Distinct().ToList();
            if (set == null || set.Count == 0) return new List<Porter>();
            return _context.Porters.Where(p => set.Contains(p.PorterId)).ToList();
        }
    }
}
