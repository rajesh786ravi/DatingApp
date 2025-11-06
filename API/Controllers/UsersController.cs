using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UsersController(DataContext context) : BaseApiController
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();
        return users;
    }

    [HttpGet("{id}")] // /api/users/3
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        // Start delay task
        var waitTask = Wait();

        // Eager loading example: include related Posts
        var userTask = context.Users
                              .Include(u => u.Posts) // eager load navigation property
                              .FirstOrDefaultAsync(u => u.Id == id);

        // Wait for both tasks to complete
        await Task.WhenAll(waitTask, userTask);

        var user = userTask.Result;

        if (user == null) return NotFound();
        return user;
    }

    // Simulated delay
    private async Task Wait()
    {
        await Task.Delay(5000);
    }
}