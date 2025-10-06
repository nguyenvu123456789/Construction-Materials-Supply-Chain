using BusinessObjects;
using DataAccess;
using Repositories.Interface;
using System.Collections.Generic;

namespace Repositories.Repositories
{
    public class ExportRepository : IExportRepository
    {
        private readonly ExportDAO _dao;

        public ExportRepository(ExportDAO dao)
        {
            _dao = dao;
        }

        public void SaveExport(Export export) => _dao.SaveExport(export);

        public Export GetExportById(int id) => _dao.GetExportById(id);

        public List<Export> GetExports() => _dao.GetExports();
    }
}
