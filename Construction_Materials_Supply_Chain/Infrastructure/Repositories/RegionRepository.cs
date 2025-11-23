using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RegionRepository : GenericRepository<Region>, IRegionRepository
    {
        public RegionRepository(ScmVlxdContext context) : base(context)
        {
        }

        public Region? GetByName(string name)
        {
            var n = name.Trim().ToLower();
            return _dbSet
                .AsNoTracking()
                .FirstOrDefault(r => r.RegionName.ToLower() == n);
        }
    }
}
