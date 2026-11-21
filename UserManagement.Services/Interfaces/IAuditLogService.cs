using System.Collections.Generic;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Interfaces;

public interface IAuditLogService
{
    void CreateLog<T>(AuditAction action, long objectId, T? objectBefore, T? objectAfter);

    AuditLog GetLog(int id);

    IEnumerable<AuditLog> GetAll();

    IEnumerable<AuditLog> GetForObject<T>(long objectId);
}