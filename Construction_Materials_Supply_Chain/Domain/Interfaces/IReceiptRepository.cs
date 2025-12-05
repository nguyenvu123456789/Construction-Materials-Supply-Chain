using Domain.Interface.Base;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IReceiptRepository : IGenericRepository<Receipt>
    {
        List<Receipt> GetReceiptsByPartnerId(int partnerId);
        Receipt GetLastReceiptByPrefix(string prefix);
        List<Receipt> GetByDateRange(DateTime from, DateTime to);
    }
}
