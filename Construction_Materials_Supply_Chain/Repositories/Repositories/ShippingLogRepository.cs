using BusinessObjects;
using DataAccess;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class ShippingLogRepository : IShippingLogRepository
    {
        public List<ShippingLog> GetAllShippingLogs() => ShippingLogDAO.GetAllShippingLogs();
        public List<ShippingLog> SearchShippingLogs(string status) => ShippingLogDAO.SearchShippingLogs(status);
    }
}
