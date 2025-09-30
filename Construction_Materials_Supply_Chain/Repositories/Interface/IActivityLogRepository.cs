using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IActivityLogRepository
    {
        List<ActivityLog> GetLogs();
        List<ActivityLog> SearchLogs(string keyword);
        void LogAction(int userId, string action, string entityName = null, int? entityId = null);
    }
}
