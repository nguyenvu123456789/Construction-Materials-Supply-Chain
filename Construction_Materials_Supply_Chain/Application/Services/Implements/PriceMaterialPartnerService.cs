using Application.Common.Pagination;
using Application.DTOs.Application.DTOs;
using Application.Services.Interfaces;
using Domain.Interface;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PriceMaterialPartnerService : IPriceMaterialPartnerService
    {
        private readonly IPriceMaterialPartnerRepository _repo;
        private readonly IPartnerRelationRepository _partnerRelationRepo;


        public PriceMaterialPartnerService(IPriceMaterialPartnerRepository repo, IPartnerRelationRepository partnerRelationRepo)
        {
            _repo = repo;
                _partnerRelationRepo = partnerRelationRepo;
        }

        public async Task<PagedResultDto<PriceMaterialPartnerDto>> GetAllAsync(PriceCatalogQueryDto query)
        {
            var q = _repo.QueryAll();

            // FILTER
            if (query.PartnerId.HasValue)
                q = q.Where(x => x.PartnerId == query.PartnerId.Value);

            if (query.MaterialId.HasValue)
                q = q.Where(x => x.MaterialId == query.MaterialId.Value);

            if (!string.IsNullOrWhiteSpace(query.Status))
                q = q.Where(x => x.Status == query.Status);

            // COUNT
            int totalCount = await q.CountAsync();

            // PAGING
            int skip = (query.PageNumber - 1) * query.PageSize;
            var data = await q
                .OrderBy(x => x.PartnerId)
                .ThenBy(x => x.MaterialId)
                .Skip(skip)
                .Take(query.PageSize)
                .Select(x => new PriceMaterialPartnerDto
                {
                    PriceMaterialPartnerId = x.PriceMaterialPartnerId,
                    PartnerId = x.PartnerId,
                    PartnerName = x.Partner.PartnerName,
                    MaterialId = x.MaterialId,
                    MaterialName = x.Material.MaterialName,
                    SellPrice = x.SellPrice,
                    DiscountPercent = x.DiscountPercent,
                    DiscountAmount = x.DiscountAmount,
                    Status = x.Status
                })
                .ToListAsync();

            return new PagedResultDto<PriceMaterialPartnerDto>
            {
                Data = data,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = (int)System.Math.Ceiling((double)totalCount / query.PageSize)
            };
        }

        public async Task UpdatePriceAsync(PriceMaterialPartnerUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.PriceMaterialPartnerId);
            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy bảng giá.");

            entity.SellPrice = dto.SellPrice ?? entity.SellPrice;
            entity.DiscountPercent = dto.DiscountPercent ?? entity.DiscountPercent;
            entity.DiscountAmount = dto.DiscountAmount ?? entity.DiscountAmount;
            if (!string.IsNullOrWhiteSpace(dto.Status))
                entity.Status = dto.Status;

            _repo.Update(entity); // repo SaveChanges synchronous
        }

        public async Task<List<PriceMaterialPartnerDto>> GetPricesForPartnerAsync(int buyerPartnerId, int sellerPartnerId)
        {
            // Lấy quan hệ buyer-seller
            var relation = await _partnerRelationRepo.GetRelationAsync(buyerPartnerId, sellerPartnerId);

            decimal discountPercent = relation?.RelationType.DiscountPercent ?? 0;
            decimal discountAmount = relation?.RelationType.DiscountAmount ?? 0;

            // Lấy bảng giá seller
            var prices = await _repo.QueryAll()
                .Where(p => p.PartnerId == sellerPartnerId)
                .Include(p => p.Material)
                .Include(p => p.Partner)
                .ToListAsync();

            var result = prices.Select(p => new PriceMaterialPartnerDto
            {
                PriceMaterialPartnerId = p.PriceMaterialPartnerId,
                PartnerId = p.PartnerId,
                PartnerName = p.Partner.PartnerName,
                MaterialId = p.MaterialId,
                MaterialName = p.Material.MaterialName,
                SellPrice = p.SellPrice,
                DiscountPercent = discountPercent,
                DiscountAmount = discountAmount,
                PriceAfterDiscount = Math.Max(0, p.SellPrice * (1 - discountPercent / 100) - discountAmount),
                Status = p.Status
            }).ToList();

            return result;
        }

    }
}
