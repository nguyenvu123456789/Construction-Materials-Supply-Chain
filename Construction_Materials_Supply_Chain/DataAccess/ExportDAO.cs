using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class ExportDAO : BaseDAO
    {
        public ExportDAO(ScmVlxdContext context) : base(context) { }

        public void SaveExport(Export export)
        {
            Context.Exports.Add(export);
            Context.SaveChanges();
        }

        public Export GetExportById(int id)
        {
            return Context.Exports
                .Include(e => e.ExportDetails)
                .FirstOrDefault(e => e.ExportId == id)!;
        }

        public List<Export> GetExports()
        {
            return Context.Exports
                .Include(e => e.ExportDetails)
                .Include(e => e.Warehouse)
                .ToList();
        }
    }
}
