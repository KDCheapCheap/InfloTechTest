using System;
using UserManagement.Data.Entities;

namespace UserManagement.Web.Models.AuditLogs;

public class ViewAuditLogViewModel
{
        public long Id { get; set; }

        public AuditAction Action { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ObjectType { get; set; } = default!;

        public long ObjectId { get; set; }

        public string? ObjectBeforeJson { get; set; }

        public string? ObjectAfterJson { get; set; }
}