using System.Collections.Generic;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Interfaces;

public interface IAuditLogService
{
    void CreateLog<T>(AuditAction action, long objectId, T? objectBefore, T? objectAfter);

    AuditLog GetLog(long id);

    IEnumerable<AuditLog> GetAll();

    IEnumerable<AuditLog> GetAllForObject<T>(long objectId);
}