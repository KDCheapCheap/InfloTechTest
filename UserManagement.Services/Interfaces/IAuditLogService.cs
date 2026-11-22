using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Interfaces;

public interface IAuditLogService
{
    Task CreateLog<T>(AuditAction action, long objectId, T? objectBefore, T? objectAfter);

    Task<AuditLog> GetLog(long id);

    Task<IEnumerable<AuditLog>> GetAll();

    Task<IEnumerable<AuditLog>> GetAllForObject<T>(long objectId);
}