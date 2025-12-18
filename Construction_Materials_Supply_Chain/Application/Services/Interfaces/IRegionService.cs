using Application.DTOs;

namespace Application.Services.Interfaces
{
    public interface IRegionService
    {
        IEnumerable<RegionDto> GetAll();
        RegionDto? GetById(int id);
        RegionDto Create(RegionCreateDto dto);
        void Update(int id, RegionUpdateDto dto);
        void Delete(int id);
        bool CanTrade(string regionNameA, string regionNameB);
        public interface IRegionService;
        IEnumerable<PartnerWithRegionsDto> GetBuyerRegions(int partnerId);
        IEnumerable<PartnerWithRegionsDto> GetSellerRegions(int partnerId);


    }
}
