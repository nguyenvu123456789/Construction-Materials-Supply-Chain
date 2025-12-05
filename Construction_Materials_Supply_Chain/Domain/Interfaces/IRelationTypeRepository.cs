using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IRelationTypeRepository : IGenericRepository<RelationType>
    {
        IQueryable<RelationType> Query();
        IQueryable<RelationType> QueryIncludeRelations();
    }
}
