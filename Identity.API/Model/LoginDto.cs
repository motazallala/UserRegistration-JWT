using System.ComponentModel.DataAnnotations;

namespace Identity.API.Model;

public class LoginDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}