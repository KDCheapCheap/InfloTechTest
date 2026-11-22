using System.Linq;
using System.Threading.Tasks;
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
    public async Task<IActionResult> List()
    {
        List<AuditLog> logs = (await _auditLogService.GetAll()).ToList();

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
    public async Task<IActionResult> List(string type)
    {
        List<AuditLog> logs = (await _auditLogService.GetAll()).Where(p => p.ObjectType == type).ToList();

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

    [HttpGet("view/{id:int}")]
    public async Task<IActionResult> View(long id)
    {
        var log = await _auditLogService.GetLog(id);

        if (log == null)
        {
            TempData["ErrorMessage"] = $"The requested Audit Log with id {id} could not be found.";
            return RedirectToAction(nameof(List));
        }

        return View(new ViewAuditLogViewModel
        {
            Id = log.Id,
            Action = log.Action,
            CreatedAt = log.CreatedAt,
            ObjectType = log.ObjectType,
            ObjectId = log.ObjectId,
            ObjectBeforeJson = log.ObjectBeforeJson,
            ObjectAfterJson = log.ObjectAfterJson
        });
    }
}