using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IInventoryAlertRuleRepository
    {
        InventoryAlertRule Add(InventoryAlertRule rule);
        void Update(InventoryAlertRule rule);
        void SetActive(int ruleId, bool isActive, int partnerId);
        InventoryAlertRule? Get(int id, int partnerId);
        List<InventoryAlertRule> GetActiveByPartner(int partnerId);
        void ReplaceRoles(int ruleId, int partnerId, IEnumerable<int> roleIds);
        void ReplaceUsers(int ruleId, int partnerId, IEnumerable<int> userIds);
    }
}
