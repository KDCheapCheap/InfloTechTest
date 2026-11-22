using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Services.Tests;

public class AuditLogServiceTests
{
    [Fact]
    public async Task CreateLog_CallsContextCreate()
    {
        AuditLog? createdLog = null;
        _dataContext
            .Setup(context => context.CreateAsync(It.IsAny<AuditLog>()))
            .Callback<AuditLog>(log => createdLog = log)
            .Returns(Task.CompletedTask);
        var service = CreateService();

        await service.CreateLog<TestObject>(AuditAction.Update, 42, null, null);

        _dataContext.Verify(context => context.CreateAsync(It.IsAny<AuditLog>()));
        createdLog.Should().NotBeNull();
        createdLog!.Action.Should().Be(AuditAction.Update);
        createdLog.ObjectType.Should().Be(nameof(TestObject));
        createdLog.ObjectId.Should().Be(42);
        createdLog.ObjectBeforeJson.Should().BeNull();
        createdLog.ObjectAfterJson.Should().BeNull();
    }

    [Fact]
    public async Task CreateLog_SerializesBeforeAndAfterObjects()
    {
        AuditLog? createdLog = null;
        _dataContext
            .Setup(context => context.CreateAsync(It.IsAny<AuditLog>()))
            .Callback<AuditLog>(log => createdLog = log)
            .Returns(Task.CompletedTask);
        var service = CreateService();

        await service.CreateLog(
            AuditAction.Update,
            42,
            new TestObject { Name = "before", Value = 1 },
            new TestObject { Name = "after", Value = 2 });

        createdLog!.ObjectBeforeJson.Should().Be("{\"Name\":\"before\",\"Value\":1}");
        createdLog.ObjectAfterJson.Should().Be("{\"Name\":\"after\",\"Value\":2}");
    }

    [Fact]
    public async Task GetLog_ReturnsContextLog()
    {
        var log = new AuditLog { Id = 42 };
        _dataContext.Setup(context => context.GetAsync<AuditLog>(42)).ReturnsAsync(log);
        var service = CreateService();

        var result = await service.GetLog(42);

        result.Should().BeSameAs(log);
    }

    [Fact]
    public async Task GetLog_WhenContextThrows_ReturnsNull()
    {
        _dataContext
            .Setup(context => context.GetAsync<AuditLog>(42))
            .ThrowsAsync(new InvalidOperationException());
        var service = CreateService();

        var result = await service.GetLog(42);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_ReturnsContextLogs()
    {
        var logs = new[] { new AuditLog(), new AuditLog() }.AsQueryable();
        _dataContext.Setup(context => context.GetAllAsync<AuditLog>()).ReturnsAsync(logs);
        var service = CreateService();

        var result = await service.GetAll();

        result.Should().BeSameAs(logs);
    }

    [Fact]
    public async Task GetAllForType_FiltersByType()
    {
        var matchingLog = new AuditLog { ObjectType = nameof(TestObject) };
        var otherLog = new AuditLog { ObjectType = "OtherObject" };
        _dataContext
            .Setup(context => context.GetAllAsync<AuditLog>())
            .ReturnsAsync(new[] { matchingLog, otherLog }.AsQueryable());
        var service = CreateService();

        var result = await service.GetAllForType<TestObject>();

        result.Should().ContainSingle().Which.Should().BeSameAs(matchingLog);
    }

    [Fact]
    public async Task GetForObject_FiltersByTypeAndObjectId()
    {
        var matchingLog = new AuditLog
        {
            ObjectType = typeof(TestObject).Name,
            ObjectId = 42
        };
        var otherLog = new AuditLog
        {
            ObjectType = typeof(TestObject).Name,
            ObjectId = 99
        };
        _dataContext
            .Setup(context => context.GetAllAsync<AuditLog>())
            .ReturnsAsync(new[] { matchingLog, otherLog }.AsQueryable());
        var service = CreateService();

        var result = await service.GetAllForObject<TestObject>(42);

        result.Should().ContainSingle().Which.Should().BeSameAs(matchingLog);
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private AuditLogService CreateService() => new(_dataContext.Object);

    private sealed class TestObject
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}