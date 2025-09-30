using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IShippingLogRepository
    {
        List<ShippingLog> GetAllShippingLogs();
        List<ShippingLog> SearchShippingLogs(string status);
    }
}
