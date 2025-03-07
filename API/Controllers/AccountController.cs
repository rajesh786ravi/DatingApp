using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context) : BaseApiController
{
    [HttpPost("register")] // account/register
    public async Task<ActionResult<AppUser>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.UserName)) return BadRequest("Username already exists.");
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = registerDTO.UserName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
    [HttpPost("unregister")] // account/unregister
    public async Task<bool> UnRegister(RegisterDTO registerDTO)
    {
        // Fetch the user from the database
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserName.ToLower() == registerDTO.UserName.ToLower());
        // Check if the user exists
        if (user == null)
        {
            return false; // User does not exist, so nothing to delete
        }
        // Verify password
        using var hmac = new HMACSHA512(user.PasswordSalt); // Use stored salt
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        // Compare computed hash with stored hash
        if (!computedHash.SequenceEqual(user.PasswordHash))
        {
            return false; // Passwords do not match
        }
        // Remove the user
        context.Users.Remove(user);
        int changes = await context.SaveChangesAsync();
        // Return true if at least one row was affected
        return changes > 0;
    }

    [HttpPost("resetpassword")] // account/resetpassword
    public async Task<bool> ResetPassword(RegisterDTO registerDTO)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == registerDTO.UserName.ToLower());
        if (user == null)
        {
            return false; // User not found
        }
        using var hmac = new HMACSHA512(); // Generate new salt
        user.PasswordSalt = hmac.Key;
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        await context.SaveChangesAsync();
        return true; // Password reset successfully
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}