using Identity.API.Model;
using Identity.Core.Model;
using Identity.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserService _userService; 
    private readonly IConfiguration _config;

    public AuthController(UserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            FullName = model.FullName
        };

        var result = await _userService.RegisterAsync(user, model.Password);

        if (!result)
        {
            return BadRequest("Username or email already exists.");
        }
        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userService.AuthenticateAsync(model.Email, model.Password);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = GenerateJwtToken(user);
        // Set the JWT token in an HTTP-only cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,  // Prevents JavaScript from accessing the cookie
            Expires = DateTime.UtcNow.AddMinutes(30), // Token expiry time
            Secure = true,   // Ensure the cookie is sent only over HTTPS
            SameSite = SameSiteMode.Strict // Helps prevent CSRF attacks
        };

        Response.Cookies.Append("jwt_token", token, cookieOptions);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
