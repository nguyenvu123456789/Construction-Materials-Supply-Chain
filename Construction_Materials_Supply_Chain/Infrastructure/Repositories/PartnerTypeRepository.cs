﻿using Infrastructure.Persistence;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class PartnerTypeRepository : GenericRepository<PartnerType>, IPartnerTypeRepository
    {
        public PartnerTypeRepository(ScmVlxdContext context) : base(context)
        {
        }
    }
}
