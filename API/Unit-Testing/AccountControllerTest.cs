using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API;
using API.Controllers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class AccountControllerTests
{
    private readonly AccountController _controller;
    private readonly Mock<EmailService> _mockEmailService;
    private readonly Mock<MyDelegateService> _mockDelegateService;
    private readonly DataContext _context;

    public AccountControllerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Use a unique in-memory DB
            .Options;

        _context = new DataContext(options);
        _mockEmailService = new Mock<EmailService>();
        _mockDelegateService = new Mock<MyDelegateService>();

        _controller = new AccountController(_context, _mockEmailService.Object, _mockDelegateService.Object);
    }

    [Fact]
    public async Task Register_ShouldCreateUser_WhenUserDoesNotExist()
    {
        // Arrange
        var registerDTO = new RegisterDTO { UserName = "testuser", Password = "password123" };

        // Act
        var result = await _controller.Register(registerDTO);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(registerDTO.UserName, result.Value.UserName);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUserAlreadyExists()
    {
        // Arrange
        var existingUser = new AppUser { UserName = "existinguser" };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var registerDTO = new RegisterDTO { UserName = "existinguser", Password = "password123" };

        // Act
        var result = await _controller.Register(registerDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_ShouldReturnUser_WhenCredentialsAreCorrect()
    {
        // Arrange
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = "validuser",
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password123")),
            PasswordSalt = hmac.Key
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDTO = new LoginDTO { UserName = "validuser", Password = "password123" };

        // Act
        var result = await _controller.Login(loginDTO);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(user.UserName, result.Value.UserName);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {
        // Arrange
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = "validuser",
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password123")),
            PasswordSalt = hmac.Key
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDTO = new LoginDTO { UserName = "validuser", Password = "wrongpassword" };

        // Act
        var result = await _controller.Login(loginDTO);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task ResetPassword_ShouldUpdatePassword_WhenUserExists()
    {
        // Arrange
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = "resetuser",
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("oldpassword")),
            PasswordSalt = hmac.Key
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var resetDTO = new RegisterDTO { UserName = "resetuser", Password = "newpassword" };

        // Act
        var result = await _controller.ResetPassword(resetDTO);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var resetDTO = new RegisterDTO { UserName = "nonexistentuser", Password = "newpassword" };

        // Act
        var result = await _controller.ResetPassword(resetDTO);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RunDelegate_ShouldCallDelegateMethod()
    {
        // Act
        var result = _controller.RunDelegate();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
