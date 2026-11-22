using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    [Fact]
    public async Task GetAll_WhenNewEntityAdded_MustIncludeNewEntityAsync()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        await context.CreateAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetAllAsync<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task GetAll_WhenDeleted_MustNotIncludeDeletedEntityAsync()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = CreateUser();
        await context.CreateAsync(entity);
        await   context.DeleteAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetAllAsync<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public async Task GetAll_WhenSoftDeleted_MustNotIncludeEntityAsync()
    {
        // Arrange
        var context = CreateContext();
        var entity = CreateUser(isDeleted: true);
        await context.CreateAsync(entity);

        // Act
        var result = await context.GetAllAsync<User>();

        // Assert
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public async Task Create_WhenNewEntityAdded_MustIncludeNewEntityAsync()
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
        await context.CreateAsync(entity);
        var result = await context.GetAllAsync<User>();

        // Assert
        result.Should().Contain(s => s.Email == entity.Email);
    }

    [Fact]
    public async Task Get_WhenEntityExists_MustReturnEntityAsync()
    {
        var context = CreateContext();
        var entity = CreateUser();
        await context.CreateAsync(entity);
        var result = await context.GetAsync<User>(entity.Id);
        result.Should().BeSameAs(entity);
    }

    [Fact]
    public async Task WhenObjectIsCreated_CreatesAuditLogForActionAsync()
    {
        var context = CreateContext();
        var user = CreateUser();

        await context.CreateAsync(user);

        context.AuditLogs.Should().Contain(log =>
            log.ObjectId == user.Id &&
            log.ObjectType == nameof(User) &&
            log.Action == AuditAction.Create);
    }

    [Fact]
    public async Task WhenObjectIsEdited_CreatesAuditLogForActionAsync()
    {
        var context = CreateContext();
        var user = CreateUser();
        await context.CreateAsync(user);

        user.Forename = "Updated";
        await context.UpdateAsync(user);

        context.AuditLogs.Should().Contain(log =>
            log.ObjectId == user.Id && log.Action == AuditAction.Update);
    }

    [Fact]
    public async Task WhenObjectIsDeleted_CreatesAuditLogForActionAsync()
    {
        var context = CreateContext();
        var user = CreateUser();
        await context.CreateAsync(user);

        await context.DeleteAsync(user);

        context.AuditLogs.Should().Contain(log =>
            log.ObjectId == user.Id && log.Action == AuditAction.Delete);
    }

    [Fact]
    public async Task WhenObjectIsSoftDeleted_CreatesAuditLogForDeleteActionAsync()
    {
        // Arrange
        var context = CreateContext();
        var user = CreateUser();
        await context.CreateAsync(user);

        // Act
        user.IsDeleted = true;
        await context.UpdateAsync(user);

        // Assert
        context.AuditLogs.Should().Contain(log =>
            log.ObjectId == user.Id &&
            log.Action == AuditAction.Delete &&
            log.ObjectBeforeJson!.Contains("\"IsDeleted\":false") &&
            log.ObjectAfterJson!.Contains("\"IsDeleted\":true"));
    }

    [Fact]
    public async Task Delete_WhenEntityDeleted_MustNotIncludeItAsync()
    {
        var context = CreateContext();
        var entity = CreateUser();
        await context.CreateAsync(entity);
        await context.DeleteAsync(entity);

        var all = await context.GetAllAsync<User>();

        all.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        var context = CreateContext();
        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        context.Create(entity);

        var result = context.GetAll<User>();

        result.Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        var context = CreateContext();
        var entity = CreateUser();
        context.Create(entity);
        context.Delete(entity);

        var result = context.GetAll<User>();

        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public void GetAll_WhenSoftDeleted_MustNotIncludeEntity()
    {
        var context = CreateContext();
        var entity = CreateUser(isDeleted: true);
        context.Create(entity);

        var result = context.GetAll<User>();

        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public void Create_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        var context = CreateContext();
        var entity = new User
        {
            Forename = "Test",
            Surname = "User",
            Email = "testuser@example.com"
        };

        context.Create(entity);
        var result = context.GetAll<User>();

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
        var context = CreateContext();
        var user = CreateUser();
        context.Create(user);

        user.IsDeleted = true;
        context.Update(user);

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
