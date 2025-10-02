using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace DataAccess
{
    public class AuditLogInterceptor : SaveChangesInterceptor
    {
        private void AddAuditLogs(DbContext? context)
        {
            if (context == null) return;

            var auditEntries = new List<AuditLog>();

            foreach (var entry in context.ChangeTracker.Entries()
                                         .Where(e => e.State == EntityState.Added ||
                                                     e.State == EntityState.Modified ||
                                                     e.State == EntityState.Deleted))
            {
                string action = entry.State.ToString();
                string entityName = entry.Entity.GetType().Name;
                int entityId = 0;

                var key = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
                if (key != null && key.CurrentValue != null)
                {
                    entityId = Convert.ToInt32(key.CurrentValue);
                }

                string? changes = null;
                if (entry.State == EntityState.Modified)
                {
                    var modifiedProps = entry.Properties
                                             .Where(p => p.IsModified)
                                             .ToDictionary(p => p.Metadata.Name, p => new
                                             {
                                                 OldValue = p.OriginalValue,
                                                 NewValue = p.CurrentValue
                                             });

                    if (modifiedProps.Any())
                    {
                        changes = JsonConvert.SerializeObject(modifiedProps);
                    }
                }

                auditEntries.Add(new AuditLog
                {
                    EntityName = entityName,
                    EntityId = entityId,
                    Action = action,
                    Changes = changes,
                    UserId = null, // TODO: lấy từ HttpContext sau khi làm JWT
                    CreatedAt = DateTime.Now
                });
            }

            if (auditEntries.Any())
            {
                context.Set<AuditLog>().AddRange(auditEntries);
            }
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            AddAuditLogs(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            AddAuditLogs(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
