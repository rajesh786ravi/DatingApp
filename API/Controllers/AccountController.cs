using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, EmailService emailService, MyDelegateService myDelegateService) : BaseApiController
{
    private readonly EmailService _emailService = emailService;

    private readonly DataContext _context = context;
    private readonly MyDelegateService _myDelegateService = myDelegateService ?? throw new ArgumentNullException(nameof(myDelegateService));

    [HttpGet("run-delegate")] // account/run-delegate
    public IActionResult RunDelegate()
    {
        _myDelegateService.Run();  // Calls the delegate function
        return Ok("Delegate executed successfully!");
    }

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
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        System.Console.WriteLine(registerDTO.UserName + "Registered successfully.");
        _emailService.SendEmail(registerDTO.UserName, "Registered successfully.");
        return user;
    }
    [HttpPost("unregister")] // account/unregister
    public async Task<bool> UnRegister(RegisterDTO registerDTO)
    {
        // Fetch the user from the database
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName.ToLower() == registerDTO.UserName.ToLower());
        // Check if the user exists
        if (user == null)
        {
            return false; // User does not exist, so nothing to delete
        }
        // Verify password
        if (user.PasswordSalt != null && user.PasswordHash != null)
        {
            using var hmac = new HMACSHA512(user.PasswordSalt); // Use stored salt
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            // Compare computed hash with stored hash
            if (!computedHash.SequenceEqual(user.PasswordHash))
            {
                return false; // Passwords do not match
            }
        }
        // Remove the user
        _context.Users.Remove(user);
        int changes = await _context.SaveChangesAsync();
        // Return true if at least one row was affected
        return changes > 0;
    }

    [HttpPost("resetpassword")] // account/resetpassword
    public async Task<bool> ResetPassword(RegisterDTO registerDTO)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == registerDTO.UserName.ToLower());
        if (user == null)
        {
            return false; // User not found
        }
        using var hmac = new HMACSHA512(); // Generate new salt
        user.PasswordSalt = hmac.Key;
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        await _context.SaveChangesAsync();
        return true; // Password reset successfully
    }

    [HttpPost("login")] // account/login
    public async Task<ActionResult<AppUser>> Login(LoginDTO loginDTO)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDTO.UserName.ToLower());
        if (user == null)
        {
            return Unauthorized("Invalid username");
        }
        if (user.PasswordSalt != null && user.PasswordHash != null)
        {
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            if (!computedHash.SequenceEqual(user.PasswordHash)) return Unauthorized("Invalid password");
        }
        return user;
    }

    private async Task<bool> UserExists(string username)
    {
        System.Console.WriteLine(username + ": Already exists.");
        return await _context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}

public class EmailService
{
    public void SendEmail(string UserName, string Message)
    {
        System.Console.WriteLine("Email sent to " + UserName + ":" + Message);
    }
}

public class MyDelegateService // Created my delegate service 
{
    public MyDelegateService() { } // Ensure this exists
    private readonly Func<int, string>? _operation;

    public MyDelegateService(Func<int, string> operation)
    {
        _operation = operation;  // Assign function
    }

    public void Run()
    {
        if (_operation != null) Console.WriteLine(_operation(5));  // Call function delegate
    }
}