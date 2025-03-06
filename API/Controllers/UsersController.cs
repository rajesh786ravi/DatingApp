using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UsersController(DataContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();
        return users;
    }

    [HttpGet("{id}")] // /api/users/3
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var users = await context.Users.FindAsync(id);
        if (users == null) return NotFound();
        return users;
    }
}