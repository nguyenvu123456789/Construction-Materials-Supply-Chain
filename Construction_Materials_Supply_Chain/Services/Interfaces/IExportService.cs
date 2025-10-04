using System.Collections.Generic;
using BusinessObjects;

namespace Services.Interfaces
{
    public interface IExportService
    {
        List<Export> GetAll();
        Export? GetById(int id);
        void Create(Export export);
    }
}
