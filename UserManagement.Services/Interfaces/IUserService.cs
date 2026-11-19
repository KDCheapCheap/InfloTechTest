using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="user"></param>
    void CreateUser(User user);

    /// <summary>
    /// Returns all users with the specified active status.
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    IEnumerable<User> FilterByActive(bool isActive);

    /// <summary>
    /// Returns all users in the system.
    /// </summary>
    /// <returns></returns>
    IEnumerable<User> GetAll();
}
