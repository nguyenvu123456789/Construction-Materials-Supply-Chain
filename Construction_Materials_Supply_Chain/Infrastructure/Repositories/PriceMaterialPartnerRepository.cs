using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PriceMaterialPartnerRepository : GenericRepository<PriceMaterialPartner>, IPriceMaterialPartnerRepository
    {
        public PriceMaterialPartnerRepository(ScmVlxdContext context) : base(context) { }

        public IQueryable<MaterialPartner> MaterialPartners()
        {
            return _context.Set<MaterialPartner>()
                .Include(x => x.Partner)
                .Include(x => x.Material).ThenInclude(m => m.Category);
        }

        public IQueryable<PriceMaterialPartner> Prices()
        {
            return _context.Set<PriceMaterialPartner>();
        }

        public void UpsertPrice(int partnerId, int materialId, decimal buyPrice, decimal sellPrice, string? status = null)
        {
            var db = _context.Set<PriceMaterialPartner>();
            var record = db.FirstOrDefault(x => x.PartnerId == partnerId && x.MaterialId == materialId);
            if (record == null)
            {
                record = new PriceMaterialPartner
                {
                    PartnerId = partnerId,
                    MaterialId = materialId,
                    BuyPrice = buyPrice,
                    SellPrice = sellPrice,
                    Status = status ?? "Active"
                };
                db.Add(record);
            }
            else
            {
                record.BuyPrice = buyPrice;
                record.SellPrice = sellPrice;
                if (!string.IsNullOrWhiteSpace(status)) record.Status = status;
                db.Update(record);
            }
            _context.SaveChanges();
        }
    }
}
