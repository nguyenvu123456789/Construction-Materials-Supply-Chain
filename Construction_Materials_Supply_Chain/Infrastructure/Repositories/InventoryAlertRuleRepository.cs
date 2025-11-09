using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class InventoryAlertRuleRepository : IInventoryAlertRuleRepository
    {
        private readonly ScmVlxdContext _ctx;
        public InventoryAlertRuleRepository(ScmVlxdContext ctx) { _ctx = ctx; }

        public InventoryAlertRule Add(InventoryAlertRule rule)
        {
            _ctx.InventoryAlertRules.Add(rule);
            _ctx.SaveChanges();
            return rule;
        }

        public void Update(InventoryAlertRule rule)
        {
            var ok = _ctx.InventoryAlertRules.Any(r => r.InventoryAlertRuleId == rule.InventoryAlertRuleId && r.PartnerId == rule.PartnerId);
            if (!ok) return;
            _ctx.InventoryAlertRules.Update(rule);
            _ctx.SaveChanges();
        }

        public void SetActive(int ruleId, bool isActive, int partnerId)
        {
            var rule = _ctx.InventoryAlertRules.FirstOrDefault(r => r.InventoryAlertRuleId == ruleId && r.PartnerId == partnerId);
            if (rule == null) return;
            rule.IsActive = isActive;
            _ctx.SaveChanges();
        }

        public InventoryAlertRule? Get(int id, int partnerId)
        {
            return _ctx.InventoryAlertRules
                .Include(r => r.Roles)
                .Include(r => r.Users)
                .FirstOrDefault(r => r.InventoryAlertRuleId == id && r.PartnerId == partnerId);
        }

        public List<InventoryAlertRule> GetActiveByPartner(int partnerId)
        {
            return _ctx.InventoryAlertRules
                .Include(r => r.Roles)
                .Include(r => r.Users)
                .Where(r => r.PartnerId == partnerId && r.IsActive)
                .ToList();
        }

        public void ReplaceRoles(int ruleId, int partnerId, IEnumerable<int> roleIds)
        {
            var rule = Get(ruleId, partnerId);
            if (rule == null) return;
            var existed = _ctx.InventoryAlertRuleRoles.Where(x => x.InventoryAlertRuleId == ruleId).ToList();
            _ctx.InventoryAlertRuleRoles.RemoveRange(existed);
            var toAdd = roleIds.Distinct().Select(id => new InventoryAlertRuleRole { InventoryAlertRuleId = ruleId, RoleId = id }).ToList();
            if (toAdd.Count > 0) _ctx.InventoryAlertRuleRoles.AddRange(toAdd);
            _ctx.SaveChanges();
        }

        public void ReplaceUsers(int ruleId, int partnerId, IEnumerable<int> userIds)
        {
            var rule = Get(ruleId, partnerId);
            if (rule == null) return;
            var existed = _ctx.InventoryAlertRuleUsers.Where(x => x.InventoryAlertRuleId == ruleId).ToList();
            _ctx.InventoryAlertRuleUsers.RemoveRange(existed);
            var valid = _ctx.Users.Where(u => userIds.Contains(u.UserId) && u.PartnerId == partnerId).Select(u => u.UserId).Distinct().ToList();
            var toAdd = valid.Select(id => new InventoryAlertRuleUser { InventoryAlertRuleId = ruleId, UserId = id }).ToList();
            if (toAdd.Count > 0) _ctx.InventoryAlertRuleUsers.AddRange(toAdd);
            _ctx.SaveChanges();
        }
    }
}
