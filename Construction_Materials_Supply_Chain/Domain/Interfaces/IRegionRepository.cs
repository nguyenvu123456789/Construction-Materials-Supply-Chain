using Domain.Interface.Base;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRegionRepository : IGenericRepository<Region>
    {
        Region? GetByName(string name);
    }
}
