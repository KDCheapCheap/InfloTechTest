using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public ViewResult List()
    {
        var users = _userService.GetAll().ToList();

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
    public ViewResult ListByActiveStatus(bool isActive)
    {
        var users = _userService.FilterByActive(isActive).ToList();

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
    public IActionResult CreateUser(CreateUserViewModel newUser)
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

        _userService.CreateUser(user);

        //Redirect back to the user list
        return RedirectToAction("List");
    }

    [HttpGet("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var user = _userService.GetById(id);
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
        };

        return View(model);
    }
    
    [HttpPost("edit/{id:int}")]
    public IActionResult Edit(int id, EditUserViewModel updatedUser)
    {
        if (!ModelState.IsValid)
        {
            return View(updatedUser);
        }

        var user = _userService.GetById(id);
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

        _userService.EditUser(user);

        return RedirectToAction("List");
    }
}
