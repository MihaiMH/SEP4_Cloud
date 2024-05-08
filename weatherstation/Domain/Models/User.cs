namespace Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ClothingPreferences { get; set; }
    public bool OnNotifications { get; set; }
    
    public User() {}
    
    public User(int id, string username, string password, string clothingPreferences, bool onNotifications)
    {
        Id = id;
        Username = username;
        Password = password;
        ClothingPreferences = clothingPreferences;
        OnNotifications = onNotifications;
    }
}