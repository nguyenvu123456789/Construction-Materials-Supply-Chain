using Domain.Interface.Base;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserOtpRepository : IGenericRepository<UserOtp>
    {
        UserOtp? GetActive(int userId, string purpose, string code);
        void InvalidateAll(int userId, string purpose);
    }
}
