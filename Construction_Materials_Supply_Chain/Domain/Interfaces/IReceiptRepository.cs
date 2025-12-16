using Domain.Interface.Base;

namespace Domain.Interfaces
{
    public interface IReceiptRepository : IGenericRepository<Receipt>
    {
        List<Receipt> GetReceiptsByPartnerId(int partnerId);
        Receipt GetLastReceiptByPrefix(string prefix);
        List<Receipt> GetByDateRange(DateTime from, DateTime to);
        List<Receipt> GetByDateRangeAndPartner(DateTime from, DateTime to, int? partnerId);
    }
}
