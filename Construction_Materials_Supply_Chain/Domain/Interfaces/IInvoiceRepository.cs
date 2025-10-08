using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Invoice? GetByCode(string code);
    }
}
