using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers(activeUser);

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }
    
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainDateOfBirth()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        SetupUsers(activeUser);

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle()
            .Which.DateOfBirth.Should().Be(activeUser.DateOfBirth);
    }

    [Fact]
    public void ListByActiveStatus_WhenIsActiveTrue_ReturnsOnlyActiveUsers()
    {
        //Arrange
        var controller = CreateController();
        _userService.Setup(service => service.FilterByActive(true)).Returns(new[] { activeUser });

        var users = SetupUsers(activeUser, inactiveUser);

        //Act
        var result = controller.ListByActiveStatus(isActive: true);

        //Assert
        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle()
            .Which.Email.Should().Be(activeUser.Email);
    }

    [Fact]
    public void ListByActiveStatus_WhenIsActiveFalse_ReturnsOnlyInactiveUsers()
    {
        //Arrange
        var controller = CreateController();
        _userService.Setup(service => service.FilterByActive(false)).Returns(new[] { inactiveUser });

        var users = SetupUsers(activeUser, inactiveUser);

        //Act
        var result = controller.ListByActiveStatus(isActive: false);

        //Assert
        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle()
            .Which.Email.Should().Be(inactiveUser.Email);
    }

    
    [Fact]
    public void ListByActiveStatus_ReturnsDateOfBirth()
    {
        //Arrange
        var controller = CreateController();
        _userService.Setup(service => service.FilterByActive(false)).Returns(new[] { inactiveUser });

        var users = SetupUsers(activeUser, inactiveUser);

        //Act
        var result = controller.ListByActiveStatus(isActive: false);

        //Assert
        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().ContainSingle()
            .Which.DateOfBirth.Should().Be(new System.DateTime(1984, 10, 28));
    }

    [Fact]
    public void View_WhenUserExists_ReturnsView()
    {
        const long userId = 42;
        var user = new User { Id = userId };
        _userService.Setup(service => service.GetById(userId)).Returns(user);
        var controller = CreateController();

        var result = controller.View(userId);

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void CreateUser_WhenModelIsValid_CreatesUser(){
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
        var result = controller.CreateUser(newUser);

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
            .Returns(testUsers);

        return testUsers;
    }

    private readonly User activeUser = new()
    {
        Forename = "Marcus",
        Surname = "Fenix",
        Email = "marcus.fenix@gow.example",
        DateOfBirth = new System.DateTime(1984, 11, 7),
        IsActive = true
    };

    private readonly User inactiveUser = new()
    {
        Forename = "Carlos",
        Surname = "Santiago",
        Email = "carlos.santiago@gow.example",
        DateOfBirth = new System.DateTime(1984, 10, 28),
        IsActive = false
    };

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}
