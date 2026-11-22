using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;

    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    public async Task CreateUser(User user)
    {
        await _dataAccess.CreateAsync(user);
    }


    public async Task EditUser(User user) 
    {
        await _dataAccess.UpdateAsync(user);
    }

    public async Task DeleteUser(User user)
    {
        user.IsDeleted = true;

        await _dataAccess.UpdateAsync(user);
    }

    public async Task<IEnumerable<User>> FilterByActive(bool isActive)
    {
        var allUsers = await _dataAccess.GetAllAsync<User>();
        return allUsers.Where(u => u.IsActive == isActive);
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _dataAccess.GetAllAsync<User>();
    }

    public async Task<User> GetById(long id)
    {
        try
        {
            var user = await _dataAccess.GetAsync<User>(id);
            return user;
        }
        catch
        {
            return null!;
        }
    }
}
