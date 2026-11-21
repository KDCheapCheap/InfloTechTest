using System.Linq;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Services.Tests;

public class AuditLogServiceTests
{
    [Fact]
    public void CreateLog_CallsContextCreate()
    {
        var service = CreateService();

        service.CreateLog<TestObject>(AuditAction.Update, 42, null, null);

        _dataContext.Verify(context => context.Create(It.IsAny<AuditLog>()));
    }

    [Fact]
    public void GetForObject_FiltersByTypeAndObjectId()
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
            .Setup(context => context.GetAll<AuditLog>())
            .Returns(new[] { matchingLog, otherLog }.AsQueryable());
        var service = CreateService();

        var result = service.GetForObject<TestObject>(42);

        result.Should().ContainSingle().Which.Should().BeSameAs(matchingLog);
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private AuditLogService CreateService() => new(_dataContext.Object);

    private sealed class TestObject
    {
    }
}