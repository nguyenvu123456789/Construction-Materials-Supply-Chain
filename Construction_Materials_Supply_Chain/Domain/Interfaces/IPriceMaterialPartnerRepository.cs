using Domain.Interface.Base;
using Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPriceMaterialPartnerRepository : IGenericRepository<PriceMaterialPartner>
    {
        IQueryable<PriceMaterialPartner> QueryAll();         
        Task<PriceMaterialPartner?> GetByIdAsync(int id);    
    }
}
