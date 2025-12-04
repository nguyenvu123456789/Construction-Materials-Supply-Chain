using Application.DTOs.Relations;

namespace Application.Interfaces
{
    public interface IPartnerRelationService
    {
        IEnumerable<PartnerRelationDto> GetAll();
        PartnerRelationDto? GetById(int id);
        PartnerRelationDto Create(PartnerRelationCreateDto dto);
        void Update(int id, PartnerRelationUpdateDto dto);
        void Delete(int id);
        IEnumerable<PartnerRelationDto> GetByBuyer(int buyerId);
        IEnumerable<PartnerRelationDto> GetBySeller(int sellerId);

    }
}
