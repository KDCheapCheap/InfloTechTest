using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.AuditLogs;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IAuditLogService _auditLogService;

    public UsersController(IUserService userService, IAuditLogService auditLogService) => (_userService, _auditLogService) = (userService, auditLogService);

    [HttpGet]
    public async Task<ViewResult> List()
    {
        var users = (await _userService.GetAll()).ToList();

        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            DateOfBirth = p.DateOfBirth,
            IsActive = p.IsActive,
        });

        var model = new UserListViewModel { Items = items.ToList() };

        return View(model);
    }

    [HttpGet("active/{isActive:bool}")]
    public async Task<ViewResult> ListByActiveStatus(bool isActive)
    {
        var users = (await _userService.FilterByActive(isActive)).ToList();

        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            DateOfBirth = p.DateOfBirth,
            IsActive = p.IsActive,
        });

        var model = new UserListViewModel { Items = items.ToList() };

        return View("List", model);
    }

    [HttpGet("create")]
    public ViewResult Create()
    {
        return View();
    }

    [HttpPost("CreateUser")]
    [Route("CreateUser")]
    public async Task<IActionResult> CreateUser(CreateUserViewModel newUser)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", newUser);
        }

        var user = new User()
        {
            Forename = newUser.Forename,
            Surname = newUser.Surname,
            Email = newUser.Email,
            DateOfBirth = newUser.DateOfBirth!.Value,
            IsActive = newUser.IsActive,
        };

        await _userService.CreateUser(user);

        //Redirect back to the user list
        return RedirectToAction("List");
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, string returnUrl = "List")
    {
        var user = await _userService.GetById(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = $"The requested user with id {id} could not be found.";
            return RedirectToAction(nameof(List));
        }

        var model = new EditUserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
            ReturnUrl = NormalizeReturnUrl(returnUrl)
        };

        return View(model); // maybe pass returnUrl too
    }

    [HttpPost("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, EditUserViewModel updatedUser)
    {
        updatedUser.Id = id;
        updatedUser.ReturnUrl = NormalizeReturnUrl(updatedUser.ReturnUrl);

        if (!ModelState.IsValid)
        {
            return View(updatedUser);
        }

        var user = await _userService.GetById(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = $"The requested user with id {id} could not be found.";
            return RedirectToAction(nameof(List));
        }

        user.Forename = updatedUser.Forename;
        user.Surname = updatedUser.Surname;
        user.Email = updatedUser.Email;
        user.DateOfBirth = updatedUser.DateOfBirth!.Value;
        user.IsActive = updatedUser.IsActive;

        await _userService.EditUser(user);

        return updatedUser.ReturnUrl == nameof(View)
            ? RedirectToAction(nameof(View), new { id })
            : RedirectToAction(nameof(List));
    }

    [HttpGet("view/{id:int}")]
    public async Task<IActionResult> View(int id)
    {
        var user = await _userService.GetById(id);

        if (user == null)
        {
            TempData["ErrorMessage"] = $"The requested user with id {id} could not be found.";
            return RedirectToAction(nameof(List));
        }

        var auditLogs = await _auditLogService.GetAllForObject<User>(user.Id);

        var auditLogsModel = auditLogs.Select(al => new AuditLogsItemViewModel()
        {
            Id = al.Id,
            Action = al.Action,
            CreatedAt = al.CreatedAt,
            ObjectType = al.ObjectType,
            ObjectId = al.ObjectId
        });

        var auditLogsList = new AuditLogsListViewModel { Items = auditLogsModel.ToList() };

        var model = new ViewUserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
            AuditLogs = auditLogsList
        };

        return View(model);
    }

    [HttpPost("delete/{id:int}")]
    public async Task<IActionResult> Delete(long id)
    {
        var user = await _userService.GetById(id);

        if (user == null)
        {
            TempData["ErrorMessage"] = $"The requested user with id {id} could not be found.";
            return RedirectToAction(nameof(List));
        }

        await _userService.DeleteUser(user);

        return RedirectToAction(nameof(List));
    }

    private static string NormalizeReturnUrl(string? returnUrl)
    {
        if (returnUrl == null || !returnUrl.Equals("View"))
        {
            return "List";
        }

        return "View";
    }
}
