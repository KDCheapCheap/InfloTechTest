using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class AuditLogService : IAuditLogService
{
    private readonly IDataContext _dataContext;

    public AuditLogService(IDataContext dataContext) => _dataContext = dataContext;

    public async Task CreateLog<T>(AuditAction action, long objectId, T? objectBefore, T? objectAfter)
    {
        await Task.Run(() => _dataContext.CreateAsync(new AuditLog
        {
            Action = action,
            ObjectType = typeof(T).Name,
            ObjectId = objectId,
            ObjectBeforeJson = objectBefore is null ? null : JsonSerializer.Serialize(objectBefore),
            ObjectAfterJson = objectAfter is null ? null : JsonSerializer.Serialize(objectAfter)
        }));

        await Task.CompletedTask;
    }

    public async Task<AuditLog> GetLog(long id)
    {
        try
        {
            var log = await _dataContext.GetAsync<AuditLog>(id);
            return log;
        }
        catch
        {
            return null!;
        }
    }

    public async Task<IEnumerable<AuditLog>> GetAll()
    {
        return await _dataContext.GetAllAsync<AuditLog>();
    }

    public async Task<IEnumerable<AuditLog>> GetAllForType<T>()
    {
        var allLogs = await _dataContext.GetAllAsync<AuditLog>();
        return allLogs.Where(log => log.ObjectType == typeof(T).Name);
    }

    public async Task<IEnumerable<AuditLog>> GetAllForObject<T>(long objectId)
    {
        var allLogs = await _dataContext.GetAllAsync<AuditLog>();
        return allLogs.Where(log => log.ObjectType == typeof(T).Name && log.ObjectId == objectId);
    }
}