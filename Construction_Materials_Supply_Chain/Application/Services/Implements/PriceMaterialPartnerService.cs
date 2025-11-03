using Application.Common.Pagination;
using Application.DTOs.Application.DTOs;
using Application.DTOs.Common.Pagination;
using Application.Services.Interfaces;
using Domain.Interfaces;

namespace Application.Services.Implements
{
    public class PriceMaterialPartnerService : IPriceMaterialPartnerService
    {
        private readonly IPriceMaterialPartnerRepository _repo;

        public PriceMaterialPartnerService(IPriceMaterialPartnerRepository repo)
        {
            _repo = repo;
        }

        public PagedResultDto<PriceMaterialPartnerDto> GetAll(PriceCatalogQueryDto query)
        {
            var baseQuery =
                from mp in _repo.MaterialPartners()
                join pr in _repo.Prices()
                    on new { mp.PartnerId, mp.MaterialId } equals new { pr.PartnerId, pr.MaterialId } into gj
                from price in gj.DefaultIfEmpty()
                select new PriceMaterialPartnerDto
                {
                    PartnerId = mp.PartnerId,
                    PartnerName = mp.Partner.PartnerName,
                    MaterialId = mp.MaterialId,
                    MaterialCode = mp.Material.MaterialCode,
                    MaterialName = mp.Material.MaterialName,
                    CategoryName = mp.Material.Category != null ? mp.Material.Category.CategoryName : "",
                    BuyPrice = price != null ? price.BuyPrice : 0m,
                    SellPrice = price != null ? price.SellPrice : 0m,
                    Status = price != null ? price.Status : "Active"
                };

            if (query.PartnerId is not null) baseQuery = baseQuery.Where(x => x.PartnerId == query.PartnerId);
            if (query.MaterialId is not null) baseQuery = baseQuery.Where(x => x.MaterialId == query.MaterialId);
            if (query.CategoryId is not null) baseQuery = baseQuery.Where(x => x.CategoryName != "" && x.CategoryName != null).Where(x => x.CategoryName == x.CategoryName).Where(x => true); // giữ nguyên, filter thực tế theo CategoryId ở dưới

            if (query.CategoryId is not null)
            {
                var q2 =
                    from mp in _repo.MaterialPartners()
                    where mp.Material.CategoryId == query.CategoryId
                    select new { mp.PartnerId, mp.MaterialId };
                baseQuery =
                    from row in baseQuery
                    join k in q2 on new { row.PartnerId, row.MaterialId } equals new { k.PartnerId, k.MaterialId }
                    select row;
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var s = query.SearchTerm.Trim();
                baseQuery = baseQuery.Where(x =>
                    x.MaterialCode.Contains(s) ||
                    x.MaterialName.Contains(s) ||
                    x.PartnerName.Contains(s));
            }

            var sortBy = (query.SortBy ?? "materialname").ToLower();
            var desc = (query.SortDir ?? "asc").ToLower() == "desc";

            baseQuery = sortBy switch
            {
                "buyprice" => desc ? baseQuery.OrderByDescending(x => x.BuyPrice ?? 0) : baseQuery.OrderBy(x => x.BuyPrice ?? 0),
                "sellprice" => desc ? baseQuery.OrderByDescending(x => x.SellPrice ?? 0) : baseQuery.OrderBy(x => x.SellPrice ?? 0),
                "partnername" => desc ? baseQuery.OrderByDescending(x => x.PartnerName) : baseQuery.OrderBy(x => x.PartnerName),
                _ => desc ? baseQuery.OrderByDescending(x => x.MaterialName) : baseQuery.OrderBy(x => x.MaterialName)
            };

            var page = query.PageNumber <= 0 ? 1 : query.PageNumber;
            var size = query.PageSize <= 0 ? 10 : query.PageSize;

            var total = baseQuery.Count();
            var items = baseQuery.Skip((page - 1) * size).Take(size).ToList();

            return new PagedResultDto<PriceMaterialPartnerDto>
            {
                Data = items,
                TotalCount = total,
                PageNumber = page,
                PageSize = size,
                TotalPages = (int)Math.Ceiling(total / (double)size)
            };
        }

        public void UpdatePrice(PriceMaterialPartnerUpdateDto dto)
        {
            _repo.UpsertPrice(dto.PartnerId, dto.MaterialId, dto.BuyPrice, dto.SellPrice, dto.Status);
        }
    }
}
