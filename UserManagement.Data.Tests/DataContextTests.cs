using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    [Fact]
    public void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        context.Create(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = CreateUser();
        context.Create(entity);
        context.Delete(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public void GetAll_WhenSoftDeleted_MustNotIncludeEntity()
    {
        // Arrange
        var context = CreateContext();
        var entity = CreateUser(isDeleted: true);
        context.Create(entity);

        // Act
        var result = context.GetAll<User>();

        // Assert
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public void Create_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange
        var context = CreateContext();
        var entity = new User
        {
            Forename = "Test",
            Surname = "User",
            Email = "testuser@example.com"
        };

        // Act
        context.Create(entity);
        var result = context.GetAll<User>();

        // Assert
        result.Should().Contain(s => s.Email == entity.Email);
    }

    [Fact]
    public void Get_WhenEntityExists_MustReturnEntity()
    {
        var context = CreateContext();
        var entity = CreateUser();
        context.Create(entity);
        context.Get<User>(entity.Id).Should().BeSameAs(entity);
    }

    [Fact]
    public void WhenObjectIsCreated_CreatesAuditLogForAction()
    {
        var context = CreateContext();
        var user = CreateUser();

        context.Create(user);

        context.AuditLogs.Should().Contain(log =>
            log.ObjectId == user.Id &&
            log.ObjectType == nameof(User) &&
            log.Action == AuditAction.Create);
    }

    [Fact]
    public void WhenObjectIsEdited_CreatesAuditLogForAction()
    {
        var context = CreateContext();
        var user = CreateUser();
        context.Create(user);

        user.Forename = "Updated";
        context.Update(user);

        context.AuditLogs.Should().Contain(log =>
            log.ObjectId == user.Id && log.Action == AuditAction.Update);
    }

    [Fact]
    public void WhenObjectIsDeleted_CreatesAuditLogForAction()
    {
        var context = CreateContext();
        var user = CreateUser();
        context.Create(user);

        context.Delete(user);

        context.AuditLogs.Should().Contain(log =>
            log.ObjectId == user.Id && log.Action == AuditAction.Delete);
    }

    [Fact]
    public void WhenObjectIsSoftDeleted_CreatesAuditLogForDeleteAction()
    {
        // Arrange
        var context = CreateContext();
        var user = CreateUser();
        context.Create(user);

        // Act
        user.IsDeleted = true;
        context.Update(user);

        // Assert
        context.AuditLogs.Should().Contain(log =>
            log.ObjectId == user.Id &&
            log.Action == AuditAction.Delete &&
            log.ObjectBeforeJson!.Contains("\"IsDeleted\":false") &&
            log.ObjectAfterJson!.Contains("\"IsDeleted\":true"));
    }

    [Fact]
    public void Delete_WhenEntityDeleted_MustNotIncludeIt()
    {
        var context = CreateContext();
        var entity = CreateUser();
        context.Create(entity);
        context.Delete(entity);
        context.GetAll<User>().Should().NotContain(s => s.Email == entity.Email);
    }

    private static User CreateUser(string forename = "Audit", string surname = "Test", string email = "", bool isDeleted = false) => new()
    {
        Forename = forename,
        Surname = surname,
        Email = email == "" ? $"audit-{Guid.NewGuid():N}@example.com" : email,
        IsDeleted = isDeleted
    };

    private DataContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }
}
