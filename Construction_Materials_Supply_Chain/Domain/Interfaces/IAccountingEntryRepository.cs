using Domain.Interface.Base;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAccountingEntryRepository : IGenericRepository<AccountingEntry>
    {
        List<AccountingEntry> GetEntriesByDocumentType(string documentType);
    }
}
