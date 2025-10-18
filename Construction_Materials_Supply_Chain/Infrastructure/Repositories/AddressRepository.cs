using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(ScmVlxdContext context) : base(context) { }

        public Address? GetByName(string name) =>
            _context.Addresses.AsNoTracking().FirstOrDefault(x => x.Name == name);

        public List<Address> Search(string? q, string? city, int? top)
        {
            var s = _context.Addresses.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                s = s.Where(x => x.Name.Contains(q) || (x.Line1 ?? "").Contains(q));
            if (!string.IsNullOrWhiteSpace(city))
                s = s.Where(x => x.City == city);
            if (top.HasValue && top.Value > 0)
                s = s.Take(top.Value);
            return s.OrderBy(x => x.Name).ToList();
        }
    }
}
