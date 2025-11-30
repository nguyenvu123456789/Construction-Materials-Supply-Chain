using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Persistence.Interceptors
{
    public sealed class AuditLogInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUser;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        public AuditLogInterceptor(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is DbContext ctx) CaptureAuditLogs(ctx);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is DbContext ctx) CaptureAuditLogs(ctx);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void CaptureAuditLogs(DbContext ctx)
        {
            var entries = ctx.ChangeTracker
                .Entries()
                .Where(e => e.Entity is not AuditLog && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToList();

            if (entries.Count == 0) return;

            var auditLogs = new List<AuditLog>();

            foreach (var entry in entries)
            {
                var entityName = entry.Metadata.ClrType.Name;
                var (action, changesJson, entityId) = BuildAudit(entry);
                if (action == null) continue;

                auditLogs.Add(new AuditLog
                {
                    EntityName = entityName,
                    EntityId = entityId,
                    Action = action,
                    Changes = changesJson,
                    CreatedAt = DateTime.Now,
                    UserId = _currentUser.UserId
                });
            }

            if (auditLogs.Count > 0)
            {
                ctx.Set<AuditLog>().AddRange(auditLogs);
            }
        }

        private static (string? Action, string? ChangesJson, int EntityId) BuildAudit(EntityEntry entry)
        {
            string? action = null;
            object? changes = null;
            var entityId = TryGetPrimaryKeyAsInt(entry);

            switch (entry.State)
            {
                case EntityState.Added:
                    action = "CREATE";
                    changes = new
                    {
                        NewValues = entry.Properties
                            .Where(p => !p.Metadata.IsPrimaryKey())
                            .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue)
                    };
                    break;

                case EntityState.Modified:
                    var modifiedProps = entry.Properties
                        .Where(p => p.IsModified && !p.Metadata.IsPrimaryKey())
                        .Select(p => new
                        {
                            Name = p.Metadata.Name,
                            Old = p.OriginalValue,
                            New = p.CurrentValue
                        })
                        .ToList();

                    if (modifiedProps.Count == 0)
                        return (null, null, entityId);

                    action = "UPDATE";
                    changes = new
                    {
                        Changes = modifiedProps
                            .ToDictionary(x => x.Name, x => new { Old = x.Old, New = x.New })
                    };
                    break;

                case EntityState.Deleted:
                    action = "DELETE";
                    changes = new
                    {
                        OldValues = entry.Properties
                            .Where(p => !p.Metadata.IsPrimaryKey())
                            .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue)
                    };
                    break;
            }

            var json = changes != null ? JsonSerializer.Serialize(changes, _jsonOptions) : null;
            return (action, json, entityId);
        }

        private static int TryGetPrimaryKeyAsInt(EntityEntry entry)
        {
            var key = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
            if (key == null || key.CurrentValue is null) return 0;

            if (key.CurrentValue is int i) return i;
            if (int.TryParse(key.CurrentValue.ToString(), out var parsed)) return parsed;

            return 0;
        }
    }
}
