using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;

    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    public void CreateUser(User user)
    {
        _dataAccess.Create(user);
    }


    public void EditUser(User user) 
    {
        _dataAccess.Update(user);
    }

    public void DeleteUser(User user)
    {
        _dataAccess.Delete(user);
    }

    public IEnumerable<User> FilterByActive(bool isActive)
    {
        return _dataAccess.GetAll<User>().Where(u => u.IsActive == isActive);
    }

    public IEnumerable<User> GetAll()
    {
        return _dataAccess.GetAll<User>();
    }

    public User GetById(long id)
    {
        try
        {
            var user = _dataAccess.Get<User>(id);
            return user;
        }
        catch
        {
            return null!;
        }
    }
}
