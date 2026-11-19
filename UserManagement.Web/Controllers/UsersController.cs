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
}
