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
    /// Edits an existing user in the system.
    /// </summary>
    /// <param name="user"></param>
    void EditUser(User user);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="id"></param>
    void DeleteUser(User user);

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

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    User GetById(long id);
}
