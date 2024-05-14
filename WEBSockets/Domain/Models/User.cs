namespace WEBSockets.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Preferences { get; set; }
        public string Email { get; set; }
        public bool OnNotifications { get; set; }

        public User() { }

        public User(string firstname, string lastName, string password, string email, bool onNotifications, string preferences)
        {
            FirstName = firstname;
            LastName = lastName;
            Password = password;
            Email = email;
            OnNotifications = onNotifications;
            Preferences = preferences;
        }
    }
}
