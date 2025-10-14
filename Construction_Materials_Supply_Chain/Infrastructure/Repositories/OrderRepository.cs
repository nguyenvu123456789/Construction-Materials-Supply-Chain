using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using System;

namespace Infrastructure.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ScmVlxdContext context) : base(context) { }
    }
}
