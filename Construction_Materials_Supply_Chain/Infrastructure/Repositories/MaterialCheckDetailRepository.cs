using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class MaterialCheckDetailRepository : IMaterialCheckDetailRepository
    {
        private readonly ScmVlxdContext _context;
        private readonly DbSet<MaterialCheckDetail> _db;

        public MaterialCheckDetailRepository(ScmVlxdContext context)
        {
            _context = context;
            _db = context.Set<MaterialCheckDetail>();
        }

        public IQueryable<MaterialCheckDetail> GetAll()
        {
            return _db.AsQueryable();
        }

        public async Task AddAsync(MaterialCheckDetail entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
