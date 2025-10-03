//using DataAccess;
//using Repositories.Interface;

//namespace Repositories.Repositories
//{
//    public class ExportRequestRepository : IExportRequestRepository
//    {
//        private readonly ExportRequestDAO _dao;

//        public ExportRequestRepository(ExportRequestDAO dao)
//        {
//            _dao = dao;
//        }

//        public List<Export> GetAll() => _dao.GetAll();

//        public Export? GetById(int id) => _dao.GetById(id);

//        public Export CreateExport(Export request) => _dao.CreateExport(request);
//    }
//}
