using Identity.Core.Interfaces;
using Identity.Core.Model;
using Identity.EF.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Services;
public class UserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    public UserService(AppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }
    public async Task<bool> RegisterAsync(User user, string password)
    {
        // Check for existing user
        if (await _context.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email))
        {
            return false; // Username or email already exists
        }

        // Hash the password and save the user
        user.PasswordHash = _passwordHasher.HashPassword(password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<User> AuthenticateAsync(string email, string password)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, password))
        {
            return null; // Invalid username or password
        }

        return user;
    }
}
