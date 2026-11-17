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
    public void List_WhenIsActiveTrue_ReturnsOnlyActiveUsers()
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
    public void List_WhenIsActiveFalse_ReturnsOnlyInactiveUsers()
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
        IsActive = true
    };

    private readonly User inactiveUser = new()
    {
        Forename = "Carlos",
        Surname = "Santiago",
        Email = "carlos.santiago@gow.example",
        IsActive = false
    };

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}
