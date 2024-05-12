namespace WEBSockets.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool OnNotifications { get; set; }

        public User() { }

        public User(string username, string password, string email, bool onNotifications)
        {
            Username = username;
            Password = password;
            Email = email;
            OnNotifications = onNotifications;
        }
    }

}
