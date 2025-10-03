//using BusinessObjects;
//using Microsoft.EntityFrameworkCore;

//namespace DataAccess
//{
//    public class ImportRequestDAO : BaseDAO
//    {
//        public ImportRequestDAO(ScmVlxdContext context) : base(context) { }

//        public List<Import> GetAll()
//        {
//            return Context.ImportRequests
//                          .Include(r => r.Details)
//                          .ThenInclude(d => d.Material)
//                          .ToList();
//        }

//        public Import? GetById(int id)
//        {
//            return Context.ImportRequests
//                          .Include(r => r.Details)
//                          .ThenInclude(d => d.Material)
//                          .FirstOrDefault(r => r.ImportRequestId == id);
//        }

//        public Import CreateImport(Import request)
//        {
//            using var transaction = Context.Database.BeginTransaction();
//            try
//            {
//                request.RequestDate = DateTime.Now;
//                Context.ImportRequests.Add(request);
//                Context.SaveChanges();

//                foreach (var detail in request.Details)
//                {
//                    var inventory = Context.Inventories
//                        .FirstOrDefault(i => i.WarehouseId == request.WarehouseId
//                                          && i.MaterialId == detail.MaterialId);

//                    if (inventory != null)
//                    {
//                        inventory.Quantity = (inventory.Quantity ?? 0) + (int)detail.Quantity;
//                        inventory.UpdatedAt = DateTime.Now;
//                    }
//                    else
//                    {
//                        Context.Inventories.Add(new Inventory
//                        {
//                            WarehouseId = request.WarehouseId,
//                            MaterialId = detail.MaterialId,
//                            Quantity = (int)detail.Quantity,
//                            CreatedAt = DateTime.Now
//                        });
//                    }
//                }

//                Context.SaveChanges();
//                transaction.Commit();

//                return request;
//            }
//            catch
//            {
//                transaction.Rollback();
//                throw;
//            }
//        }
//    }
//}
