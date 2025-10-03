//using DataAccess;
//using Repositories.Interface;

//namespace Repositories.Repositories
//{
//    public class ImportRequestRepository : IImportRequestRepository
//    {
//        private readonly ImportRequestDAO _dao;

//        public ImportRequestRepository(ImportRequestDAO dao)
//        {
//            _dao = dao;
//        }

//        public List<Import> GetAll() => _dao.GetAll();

//        public Import? GetById(int id) => _dao.GetById(id);

//        public Import CreateImport(Import request) => _dao.CreateImport(request);
//    }
//}
