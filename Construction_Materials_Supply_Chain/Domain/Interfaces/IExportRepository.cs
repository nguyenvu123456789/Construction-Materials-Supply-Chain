using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IExportRepository : IGenericRepository<Export>
    {
        Export GetExportById(int id);
    }
}
