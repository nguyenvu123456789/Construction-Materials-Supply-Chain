using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IRegionService
    {
        IEnumerable<RegionDto> GetAll();
        RegionDto? GetById(int id);
        RegionDto Create(RegionCreateDto dto);
        void Update(int id, RegionUpdateDto dto);
        void Delete(int id);
    }
}
