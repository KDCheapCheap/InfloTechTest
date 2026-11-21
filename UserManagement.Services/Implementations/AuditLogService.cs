using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class AuditLogService : IAuditLogService
{
    private readonly IDataContext _dataContext;

    public AuditLogService(IDataContext dataContext) => _dataContext = dataContext;

    public void CreateLog<T>(AuditAction action, long objectId, T? objectBefore, T? objectAfter)
    {
        _dataContext.Create(new AuditLog
        {
            Action = action,
            ObjectType = typeof(T).Name,
            ObjectId = objectId,
            ObjectBeforeJson = objectBefore is null ? null : JsonSerializer.Serialize(objectBefore),
            ObjectAfterJson = objectAfter is null ? null : JsonSerializer.Serialize(objectAfter)
        });
    }

    public AuditLog GetLog(long id)
    {
        try
        {
            var log = _dataContext.Get<AuditLog>(id);
            return log;
        }
        catch
        {
            return null!;
        }
    }

    public IEnumerable<AuditLog> GetAll()
    {
        return _dataContext.GetAll<AuditLog>();
    }

    public IEnumerable<AuditLog> GetAllForType<T>()
    {
        return _dataContext.GetAll<AuditLog>().Where(log => log.ObjectType == typeof(T).Name);
    }

    public IEnumerable<AuditLog> GetAllForObject<T>(long objectId)
    {
        return _dataContext.GetAll<AuditLog>().Where(log => log.ObjectType == typeof(T).Name && log.ObjectId == objectId);
    }
}