namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    public DateOnly BirthDate { get; set; }
    public required string UserName { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public byte[]? PasswordHash { get; set; }
    // Navigation property: one user has many Posts
    public ICollection<Post> Posts { get; set; } = new List<Post>();
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

// Post.cs
public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}