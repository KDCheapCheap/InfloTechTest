using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;
using Microsoft.Extensions.Logging;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public async Task List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers(activeUser);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        var expectedItems = users.Select(user => new UserListItemViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive
        });

        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(expectedItems);
    }

    [Fact]
    public async Task List_WhenServiceReturnsUsers_ModelMustContainDateOfBirth()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        SetupUsers(activeUser);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle()
            .Which.DateOfBirth.Should().Be(activeUser.DateOfBirth);
    }

    [Fact]
    public async Task List_WhenIsActiveTrue_ReturnsOnlyActiveUsers()
    {
        var controller = CreateController();
        _userService.Setup(service => service.FilterByActive(true)).ReturnsAsync(new[] { activeUser });
        SetupUsers(activeUser, inactiveUser);

        var result = await controller.List(isActive: true);

        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle()
            .Which.Email.Should().Be(activeUser.Email);
    }

    [Fact]
    public async Task List_WhenIsActiveFalse_ReturnsOnlyInactiveUsers()
    {
        var controller = CreateController();
        _userService.Setup(service => service.FilterByActive(false)).ReturnsAsync(new[] { inactiveUser });
        SetupUsers(activeUser, inactiveUser);

        var result = await controller.List(isActive: false);

        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle()
            .Which.Email.Should().Be(inactiveUser.Email);
    }

    [Fact]
    public async Task ListByActiveStatus_WhenIsActiveTrue_ReturnsOnlyActiveUsers()
    {
        //Arrange
        var controller = CreateController();
        _userService.Setup(service => service.FilterByActive(true)).ReturnsAsync(new[] { activeUser });

        SetupUsers(activeUser, inactiveUser);

        //Act
        var result = await controller.ListByActiveStatus(isActive: true);

        //Assert
        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle()
            .Which.Email.Should().Be(activeUser.Email);
    }

    [Fact]
    public async Task ListByActiveStatus_WhenIsActiveFalse_ReturnsOnlyInactiveUsers()
    {
        //Arrange
        var controller = CreateController();
        _userService.Setup(service => service.FilterByActive(false)).ReturnsAsync(new[] { inactiveUser });

        SetupUsers(activeUser, inactiveUser);

        //Act
        var result = await controller.ListByActiveStatus(isActive: false);

        //Assert
        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle()
            .Which.Email.Should().Be(inactiveUser.Email);
    }

    [Fact]
    public async Task ListByActiveStatus_ReturnsDateOfBirth()
    {
        //Arrange
        var controller = CreateController();
        _userService.Setup(service => service.FilterByActive(false)).ReturnsAsync(new[] { inactiveUser });

        SetupUsers(activeUser, inactiveUser);

        //Act
        var result = await controller.ListByActiveStatus(isActive: false);

        //Assert
        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle()
            .Which.DateOfBirth.Should().Be(inactiveUser.DateOfBirth);
    }

    [Fact]
    public async Task View_WhenUserExists_ReturnsView()
    {
        const long userId = 42;
        var user = new User { Id = userId };
        _userService.Setup(service => service.GetById(userId)).ReturnsAsync(user);
        _auditLogService.Setup(service => service.GetAllForObject<User>(userId))
            .ReturnsAsync(Enumerable.Empty<AuditLog>());
        var controller = CreateController();

        var result = await controller.View((int)userId);

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task CreateUser_WhenModelIsValid_CreatesUser()
    {
        //Arrange
        var controller = CreateController();
        var newUser = new CreateUserViewModel
        {
            Forename = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new System.DateTime(1990, 1, 1),
            IsActive = true
        };

        //Act
        var result = await controller.CreateUser(newUser);

        //Assert
        _userService.Verify(service => service.CreateUser(It.Is<User>(u =>
            u.Forename == newUser.Forename &&
            u.Surname == newUser.Surname &&
            u.Email == newUser.Email &&
            u.DateOfBirth == newUser.DateOfBirth.Value &&
            u.IsActive == newUser.IsActive
        )), Times.Once);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("List");
    }

    private User[] SetupUsers(params User[] testUsers)
    {
        _userService
            .Setup(s => s.GetAll())
            .ReturnsAsync(testUsers);

        return testUsers;
    }

    private readonly User activeUser = new()
    {
        Forename = "Marcus",
        Surname = "Fenix",
        Email = "marcus.fenix@gow.example",
        DateOfBirth = new System.DateTime(1984, 11, 7),
        IsActive = true,
        IsDeleted = false
    };

    private readonly User inactiveUser = new()
    {
        Forename = "Carlos",
        Surname = "Santiago",
        Email = "carlos.santiago@gow.example",
        DateOfBirth = new System.DateTime(1984, 10, 28),
        IsActive = false,
        IsDeleted = false
    };

    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IAuditLogService> _auditLogService = new();
    private readonly Mock<ILogger<UsersController>> _logger = new();
    
    private UsersController CreateController() => new(_userService.Object, _auditLogService.Object, _logger.Object);
}
