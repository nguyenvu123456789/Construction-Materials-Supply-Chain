using BusinessObjects;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Implementations
{
    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _partners;
        private readonly IPartnerTypeRepository _partnerTypes;

        public PartnerService(IPartnerRepository partners, IPartnerTypeRepository partnerTypes)
        {
            _partners = partners;
            _partnerTypes = partnerTypes;
        }

        public List<PartnerType> GetPartnerTypesWithPartners()
        {
            var types = _partnerTypes.GetAll();
            var partners = _partners.GetAll();

            var map = types.ToDictionary(t => t.PartnerTypeId, t => new List<Partner>());
            foreach (var p in partners)
            {
                if (map.ContainsKey(p.PartnerTypeId))
                    map[p.PartnerTypeId].Add(p);
            }

            foreach (var t in types)
            {
                t.Partners = map.TryGetValue(t.PartnerTypeId, out var list) ? list : new List<Partner>();
            }

            return types;
        }

        public List<Partner> GetPartnersByType(int partnerTypeId)
        {
            return _partners.GetAll().Where(p => p.PartnerTypeId == partnerTypeId).ToList();
        }

        public List<Partner> GetPartnersFiltered(string? searchTerm, List<string>? partnerTypeNames, int pageNumber, int pageSize, out int totalCount)
        {
            var partners = _partners.GetAll().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                partners = partners.Where(p => (p.PartnerName ?? "").Contains(searchTerm));

            if (partnerTypeNames != null && partnerTypeNames.Any())
            {
                var typeIds = _partnerTypes.GetAll()
                    .Where(t => partnerTypeNames.Contains(t.TypeName ?? ""))
                    .Select(t => t.PartnerTypeId)
                    .ToHashSet();

                partners = partners.Where(p => typeIds.Contains(p.PartnerTypeId));
            }

            totalCount = partners.Count();

            if (pageNumber > 0 && pageSize > 0)
                partners = partners.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return partners.ToList();
        }
    }
}
