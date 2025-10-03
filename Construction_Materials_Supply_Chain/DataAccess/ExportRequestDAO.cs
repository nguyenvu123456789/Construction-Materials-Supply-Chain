//using BusinessObjects;
//using Microsoft.EntityFrameworkCore;

//namespace DataAccess
//{
//    public class ExportRequestDAO : BaseDAO
//    {
//        public ExportRequestDAO(ScmVlxdContext context) : base(context) { }

//        public List<Export> GetAll()
//        {
//            return Context.ExportRequests
//                          .Include(r => r.Details)
//                          .ThenInclude(d => d.Material)
//                          .ToList();
//        }

//        public Export? GetById(int id)
//        {
//            return Context.ExportRequests
//                          .Include(r => r.Details)
//                          .ThenInclude(d => d.Material)
//                          .FirstOrDefault(r => r.ExportRequestId == id);
//        }

//        public Export CreateExport(Export request)
//        {
//            using var transaction = Context.Database.BeginTransaction();
//            try
//            {
//                request.RequestDate = DateTime.Now;
//                Context.ExportRequests.Add(request);
//                Context.SaveChanges();

//                foreach (var detail in request.Details)
//                {
//                    var inventory = Context.Inventories
//                        .FirstOrDefault(i => i.WarehouseId == request.WarehouseId
//                                          && i.MaterialId == detail.MaterialId);

//                    if (inventory == null || (inventory.Quantity ?? 0) < detail.Quantity)
//                    {
//                        throw new InvalidOperationException(
//                            $"Không đủ tồn kho cho MaterialId {detail.MaterialId}");
//                    }

//                    inventory.Quantity -= (int)detail.Quantity;
//                    inventory.UpdatedAt = DateTime.Now;
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
