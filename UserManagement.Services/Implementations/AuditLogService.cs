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
            ObjectType = GetObjectType<T>(),
            ObjectId = objectId,
            ObjectBeforeJson = objectBefore is null ? null : JsonSerializer.Serialize(objectBefore),
            ObjectAfterJson = objectAfter is null ? null : JsonSerializer.Serialize(objectAfter)
        });
    }

    public AuditLog GetLog(int id)
    {
        return _dataContext.Get<AuditLog>(id);
    }

    public IEnumerable<AuditLog> GetAll()
    {
        return _dataContext.GetAll<AuditLog>();
    }

    public IEnumerable<AuditLog> GetForObject<T>(long objectId)
    {
        return _dataContext.GetAll<AuditLog>()
            .Where(log => log.ObjectType == GetObjectType<T>() && log.ObjectId == objectId);
    }

    private static string GetObjectType<T>() => typeof(T).Name;
}