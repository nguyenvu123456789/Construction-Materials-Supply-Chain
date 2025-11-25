using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

public class MaterialCheckRepository : GenericRepository<MaterialCheck>, IMaterialCheckRepository
{
    private readonly ScmVlxdContext _context;

    public MaterialCheckRepository(ScmVlxdContext context) : base(context)
    {
        _context = context;
    }

    public IQueryable<MaterialCheck> GetAllWithDetails()
    {
        return _context.MaterialChecks
            .Include(c => c.Details)
                .ThenInclude(d => d.Material)
            .Include(c => c.Warehouse)
            .Include(c => c.User);
    }

}
