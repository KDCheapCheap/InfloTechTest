using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, IAuditLogService auditLogService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ViewResult> List(bool? isActive = null)
    {
        _logger.LogInformation("Listing users");

        List<User> users = new List<User>();

        if(isActive.HasValue) 
        {
            _logger.LogInformation("Filtering by active " + isActive.Value);
            users = (await _userService.FilterByActive(isActive.Value)).ToList();
        }
        else
        {
            users = (await _userService.GetAll()).ToList();
        }

        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            DateOfBirth = p.DateOfBirth.ToLocalTime(),
            IsActive = p.IsActive,
        });

        var model = new UserListViewModel { Items = items.ToList() };

        return View(model);
    }

    [HttpGet("active/{isActive:bool}")]
    public async Task<ViewResult> ListByActiveStatus(bool isActive)
    {
        _logger.LogInformation("Filtering users by active status " + isActive);

        var users = (await _userService.FilterByActive(isActive)).ToList();

        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            DateOfBirth = p.DateOfBirth.ToLocalTime(),
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
        _logger.LogInformation("Creating user");
        
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model for CreateUser");
            return View("Create", newUser);
        }

        if(newUser.DateOfBirth > System.DateTime.Now)
        {
            _logger.LogWarning("Cannot create a user with Date of birth in the future");
            ModelState.AddModelError("DateOfBirth", "Date of birth cannot be in the future");
            return View("Create", newUser);
        }

        var user = new User()
        {
            Forename = newUser.Forename,
            Surname = newUser.Surname,
            Email = newUser.Email,
            DateOfBirth = newUser.DateOfBirth!.Value.ToUniversalTime(),
            IsActive = newUser.IsActive,
        };
        
        _logger.LogInformation("Persisting user");
        
        await _userService.CreateUser(user);

        _logger.LogInformation("User created");

        //Redirect back to the user list
        return RedirectToAction("List");
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, string returnUrl = "List")
    {
        _logger.LogInformation("Fetching user for edit");

        var user = await _userService.GetById(id);
        if (user == null)
        {
            _logger.LogWarning("User with id {id} not found", id);
            TempData["ErrorMessage"] = $"The requested user with id {id} could not be found.";
            return RedirectToAction(nameof(List));
        }

        var model = new EditUserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth.ToLocalTime(),
            IsActive = user.IsActive,
            ReturnUrl = NormalizeReturnUrl(returnUrl)
        };
        _logger.LogInformation("User successfully found, returning edit view");
        return View(model); // maybe pass returnUrl too
    }

    [HttpPost("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, EditUserViewModel updatedUser)
    {
        _logger.LogInformation("Editing user");

        updatedUser.Id = id;
        updatedUser.ReturnUrl = NormalizeReturnUrl(updatedUser.ReturnUrl);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model for edit");
            return View(updatedUser);
        }

        if(updatedUser.DateOfBirth > DateTime.Now)
        {
            ModelState.AddModelError("DateOfBirth", "Date of birth cannot be in the future");
            _logger.LogWarning("Date of birth in the future for user {id}", id);
            return View(updatedUser);
        }

        var user = await _userService.GetById(id);
        if (user == null)
        {
            _logger.LogWarning("User with id {id} not found for edit", id);
            TempData["ErrorMessage"] = $"The requested user with id {id} could not be found.";
            return RedirectToAction(nameof(List));
        }

        user.Forename = updatedUser.Forename;
        user.Surname = updatedUser.Surname;
        user.Email = updatedUser.Email;
        user.DateOfBirth = updatedUser.DateOfBirth!.Value.ToUniversalTime();
        user.IsActive = updatedUser.IsActive;

        await _userService.EditUser(user);

        return updatedUser.ReturnUrl == nameof(View)
            ? RedirectToAction(nameof(View), new { id })
            : RedirectToAction(nameof(List));
    }

    [HttpGet("view/{id:int}")]
    public async Task<IActionResult> View(int id)
    {
        _logger.LogInformation("Viewing user {id}", id);
        var user = await _userService.GetById(id);

        if (user == null)
        {
            _logger.LogWarning("User with id {id} not found for view", id);
            TempData["ErrorMessage"] = $"The requested user with id {id} could not be found.";
            return RedirectToAction(nameof(List));
        }

        _logger.LogInformation("Fetching audit logs for user {id}", id);
        var auditLogs = await _auditLogService.GetAllForObject<User>(user.Id);

        var auditLogsModel = auditLogs.Select(al => new AuditLogsItemViewModel()
        {
            Id = al.Id,
            Action = al.Action,
            CreatedAt = al.CreatedAt.ToLocalTime(),
            ObjectType = al.ObjectType,
            ObjectId = al.ObjectId
        });

        var auditLogsList = new AuditLogsListViewModel { Items = auditLogsModel.ToList() };

        _logger.LogInformation("Preparing view model");
        var model = new ViewUserViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth.ToLocalTime(),
            IsActive = user.IsActive,
            AuditLogs = auditLogsList
        };

        _logger.LogInformation("Returning view for user {id}", id);
        return View(model);
    }

    [HttpPost("delete/{id:int}")]
    public async Task<IActionResult> Delete(long id)
    {
        _logger.LogInformation("Deleting user {id}", id);
        var user = await _userService.GetById(id);

        if (user == null)
        {
            _logger.LogWarning("User with id {id} not found for delete", id);
            TempData["ErrorMessage"] = $"The requested user with id {id} could not be found.";
            return RedirectToAction(nameof(List));
        }

        _logger.LogInformation("User found, proceeding to delete");
        await _userService.DeleteUser(user);
        _logger.LogInformation("User {id} deleted", id);

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
