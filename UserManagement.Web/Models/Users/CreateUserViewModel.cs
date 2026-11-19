using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Users;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "Forename is required.")]
    public string Forename { get; set; } = string.Empty;

    [Required(ErrorMessage = "Surname is required.")]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required.")]
    public DateTime? DateOfBirth { get; set; }

    public bool IsActive { get; set; }
}
