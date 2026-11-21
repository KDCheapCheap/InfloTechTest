using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Data.Interceptors;

public class DataContextSaveChangesInterceptor : SaveChangesInterceptor
{
    private List<PendingAudit>? _pendingAudits;
    private bool _savingAuditLogs;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (!_savingAuditLogs && eventData.Context is { } context)
        {
            _pendingAudits = CapturePendingAudits(context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (!_savingAuditLogs && eventData.Context is { } context)
        {
            _pendingAudits = CapturePendingAudits(context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        var pendingAudits = _pendingAudits;
        _pendingAudits = null;

        if (_savingAuditLogs || eventData.Context is not { } context || pendingAudits == null)
        {
            return base.SavedChanges(eventData, result);
        }

        SaveAuditLogs(context, pendingAudits);

        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var pendingAudits = _pendingAudits;
        _pendingAudits = null;

        if (_savingAuditLogs || eventData.Context is not { } context || pendingAudits == null)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        await SaveAuditLogsAsync(context, pendingAudits, cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _pendingAudits = null;
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _pendingAudits = null;
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private static List<PendingAudit> CapturePendingAudits(DbContext context)
    {
        context.ChangeTracker.DetectChanges();

        return context.ChangeTracker.Entries()
            .Where(entry => entry.Entity is not AuditLog &&
                (entry.State == EntityState.Added ||
                 entry.State == EntityState.Modified ||
                 entry.State == EntityState.Deleted))
            .Select(CreatePendingAudit)
            .Where(pendingAudit => pendingAudit != null)
            .Cast<PendingAudit>()
            .ToList();
    }

    private static PendingAudit? CreatePendingAudit(EntityEntry entry)
    {
        var state = IsSoftDelete(entry) ? EntityState.Deleted : entry.State;
        var beforeValues = new Dictionary<string, object?>();
        var afterValues = new Dictionary<string, object?>();
        var keyProperties = entry.Metadata.FindPrimaryKey()?.Properties;

        foreach (var property in entry.Properties)
        {
            if (keyProperties?.Contains(property.Metadata) == true)
            {
                continue;
            }

            if (state == EntityState.Modified &&
                (!property.IsModified || Equals(property.OriginalValue, property.CurrentValue)))
            {
                continue;
            }

            if (state != EntityState.Added)
            {
                beforeValues[property.Metadata.Name] = property.OriginalValue;
            }

            if (state != EntityState.Deleted || IsSoftDelete(entry))
            {
                afterValues[property.Metadata.Name] = property.CurrentValue;
            }
        }

        if (state == EntityState.Modified && beforeValues.Count == 0)
        {
            return null;
        }

        var action = state switch
        {
            EntityState.Added => AuditAction.Create,
            EntityState.Modified => AuditAction.Update,
            EntityState.Deleted => AuditAction.Delete,
            _ => throw new InvalidOperationException($"Unsupported entity state: {state}")
        };

        return new PendingAudit(
            entry,
            action,
            state == EntityState.Added ? null : GetEntityId(entry),
            entry.Metadata.ClrType.Name,
            state == EntityState.Added ? null : JsonSerializer.Serialize(beforeValues),
            state == EntityState.Deleted && !IsSoftDelete(entry) ? null : JsonSerializer.Serialize(afterValues));
    }

    private static bool IsSoftDelete(EntityEntry entry)
    {
        if (entry.State != EntityState.Modified ||
            entry.Metadata.FindProperty(nameof(User.IsDeleted)) is not { } property)
        {
            return false;
        }

        var isDeleted = entry.Property(property.Name);
        return Equals(isDeleted.OriginalValue, false) && Equals(isDeleted.CurrentValue, true);
    }

    private void SaveAuditLogs(DbContext context, IEnumerable<PendingAudit> pendingAudits)
    {
        AddAuditLogs(context, pendingAudits);

        _savingAuditLogs = true;
        try
        {
            context.SaveChanges();
        }
        finally
        {
            _savingAuditLogs = false;
        }
    }

    private async Task SaveAuditLogsAsync(
        DbContext context,
        IEnumerable<PendingAudit> pendingAudits,
        CancellationToken cancellationToken)
    {
        AddAuditLogs(context, pendingAudits);

        _savingAuditLogs = true;
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _savingAuditLogs = false;
        }
    }

    private static void AddAuditLogs(DbContext context, IEnumerable<PendingAudit> pendingAudits)
    {
        foreach (var pendingAudit in pendingAudits)
        {
            context.Set<AuditLog>().Add(new AuditLog
            {
                Action = pendingAudit.Action,
                ObjectType = pendingAudit.ObjectType,
                ObjectId = pendingAudit.ObjectId ?? GetEntityId(pendingAudit.Entry),
                ObjectBeforeJson = pendingAudit.ObjectBeforeJson,
                ObjectAfterJson = pendingAudit.ObjectAfterJson
            });
        }
    }

    private static long GetEntityId(EntityEntry entity)
    {
        var keyProperties = entity.Metadata.FindPrimaryKey()?.Properties;

        if (keyProperties == null || keyProperties.Count != 1)
        {
            throw new InvalidOperationException("Audit logging requires a single-column primary key.");
        }

        return Convert.ToInt64(entity.Property(keyProperties[0].Name).CurrentValue);
    }

    private sealed record PendingAudit(
        EntityEntry Entry,
        AuditAction Action,
        long? ObjectId,
        string ObjectType,
        string? ObjectBeforeJson,
        string? ObjectAfterJson);
}