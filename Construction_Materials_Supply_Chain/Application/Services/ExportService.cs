using Application.Interfaces;
using Domain.Models;
using Domain.Interface;

namespace Services.Implementations
{
    public class ExportService : IExportService
    {
        private readonly IExportRepository _repo;

        public ExportService(IExportRepository repo)
        {
            _repo = repo;
        }

        public List<Export> GetAll() => _repo.GetAll();

        public Export? GetById(int id) => _repo.GetById(id);

        public void Create(Export export) => _repo.Add(export);
    }
}
