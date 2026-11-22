using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="user"></param>
    Task CreateUser(User user);

    /// <summary>
    /// Edits an existing user in the system.
    /// </summary>
    /// <param name="user"></param>
    Task EditUser(User user);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="id"></param>
    Task DeleteUser(User user);

    /// <summary>
    /// Returns all users with the specified active status.
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    Task<IEnumerable<User>> FilterByActive(bool isActive);

    /// <summary>
    /// Returns all users in the system.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<User>> GetAll();

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<User> GetById(long id);
}
