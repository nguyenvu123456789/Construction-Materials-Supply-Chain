using Domain;

namespace Application.Interfaces
{
    public interface IExportService
    {
        List<Export> GetAll();
        Export? GetById(int id);
        void Create(Export export);
    }
}
