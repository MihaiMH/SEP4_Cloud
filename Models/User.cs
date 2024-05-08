public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public bool IsRegistered { get; set; }

    public User(string username, string password, string email, bool isRegistered)
    {
        Username = username;
        Password = password;
        Email = email;
        IsRegistered = isRegistered;
    }
}