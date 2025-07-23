using System;

namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    public DateOnly BirthDate { get; set; }
    public required string UserName { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public byte[]? PasswordHash { get; set; }
    public void updateAppUser(byte[] passwordSalt, byte[] passwordHash)
    {
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
    }
    public string getUserName()
    {
        return UserName;
    }
}