using System.Linq;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.AuditLogs;

namespace UserManagement.Web.Controllers;

[Route("audit-logs")]
public class AuditLogsController : Controller
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public IActionResult List()
    {
        List<AuditLog> logs = _auditLogService.GetAll().ToList();

        var items = logs.Select(p => new AuditLogsItemViewModel
        {
            Id = p.Id,
            Action = p.Action,
            CreatedAt = p.CreatedAt,
            ObjectType = p.ObjectType,
            ObjectId = p.ObjectId
        });

        var model = new AuditLogsListViewModel { Items = items.ToList() };

        return View(model);
    }

    [HttpGet("list/{type}")]
    public IActionResult List(string type)
    {
        List<AuditLog> logs = _auditLogService.GetAll().Where(p => p.ObjectType == type).ToList();
        
        var items = logs.Select(p => new AuditLogsItemViewModel
        {
            Id = p.Id,
            Action = p.Action,
            CreatedAt = p.CreatedAt,
            ObjectType = p.ObjectType,
            ObjectId = p.ObjectId
        });
        
        return View(new AuditLogsListViewModel { Items = items.ToList() });
    }
}