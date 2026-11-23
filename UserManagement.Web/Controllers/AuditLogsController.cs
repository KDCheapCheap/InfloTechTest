using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.AuditLogs;

namespace UserManagement.Web.Controllers;

[Route("audit-logs")]
public class AuditLogsController : Controller
{
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuditLogsController> _logger;

    public AuditLogsController(IAuditLogService auditLogService, ILogger<AuditLogsController> logger)
    {
        _auditLogService = auditLogService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        _logger.LogInformation("Listing audit logs");
        List<AuditLog> logs = (await _auditLogService.GetAll()).ToList();

        // Map to item view model.
        var items = logs.Select(p => new AuditLogsItemViewModel
        {
            Id = p.Id,
            Action = p.Action,
            CreatedAt = p.CreatedAt.ToLocalTime(),
            ObjectType = p.ObjectType,
            ObjectId = p.ObjectId
        });

        // Create page view model.
        var model = new AuditLogsListViewModel { Items = items.ToList() };

        return View(model);
    }

    [HttpGet("list/{type}")]
    public async Task<IActionResult> List(string type)
    {
        _logger.LogInformation("Listing audit logs for type " + type);
        List<AuditLog> logs = (await _auditLogService.GetAll()).Where(p => p.ObjectType == type).ToList();

        var items = logs.Select(p => new AuditLogsItemViewModel
        {
            Id = p.Id,
            Action = p.Action,
            CreatedAt = p.CreatedAt.ToLocalTime(),
            ObjectType = p.ObjectType,
            ObjectId = p.ObjectId
        });

        return View(new AuditLogsListViewModel { Items = items.ToList() });
    }

    [HttpGet("view/{id:int}")]
    public async Task<IActionResult> View(long id)
    {
        _logger.LogInformation("Viewing audit log " + id);
        var log = await _auditLogService.GetLog(id);

        if (log == null)
        {
            _logger.LogWarning("Audit log not found: " + id);
            TempData["ErrorMessage"] = $"The requested Audit Log with id {id} could not be found.";
            return RedirectToAction(nameof(List));
        }

        return View(new ViewAuditLogViewModel
        {
            Id = log.Id,
            Action = log.Action,
            CreatedAt = log.CreatedAt.ToLocalTime(),
            ObjectType = log.ObjectType,
            ObjectId = log.ObjectId,
            ObjectBeforeJson = log.ObjectBeforeJson,
            ObjectAfterJson = log.ObjectAfterJson
        });
    }
}