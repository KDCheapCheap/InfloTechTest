using System;
using UserManagement.Data.Entities;

namespace UserManagement.Web.Models.AuditLogs;

public class ViewAuditLogViewModel
{
        public int Id { get; set; }

        public AuditAction Action { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ObjectType { get; set; } = default!;

        public string? ObjectBeforeJson { get; set; }

        public string? ObjectAfterJson { get; set; }
}