using System;
using System.Runtime.CompilerServices;
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
        var waitTask = Wait();
        var userTask = context.Users.FindAsync(id).AsTask();
        await Task.WhenAll(waitTask, userTask);
        var users = userTask.Result;
        if (users == null) return NotFound();
        return users;
    }

    private async Task Wait()
    {
        await Task.Delay(5000);
    }
}