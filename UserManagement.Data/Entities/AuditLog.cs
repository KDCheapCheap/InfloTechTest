using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Data.Entities
{
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public AuditAction Action { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string ObjectType { get; set; } = default!;

        public long ObjectId { get; set; }

        public string? ObjectBeforeJson { get; set; }

        public string? ObjectAfterJson { get; set; }
    }

    public enum AuditAction
    {
        Create,
        Update,
        Delete
    }
}