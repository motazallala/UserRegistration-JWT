using Identity.Core.Validators;
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Model;

public class RegisterDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    [StrongPassword]
    public string Password { get; set; }
    [Required]
    public string FullName { get; set; }
}