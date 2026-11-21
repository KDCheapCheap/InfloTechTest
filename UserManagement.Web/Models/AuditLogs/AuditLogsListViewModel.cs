using System;
using UserManagement.Data.Entities;

namespace UserManagement.Web.Models.AuditLogs;

public class AuditLogsListViewModel
{
    public List<AuditLogsItemViewModel> Items { get; set; } = new();
}

public class AuditLogsItemViewModel
{
        public int Id { get; set; }

        public AuditAction Action { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ObjectType { get; set; } = default!;

        public long ObjectId { get; set; }
}