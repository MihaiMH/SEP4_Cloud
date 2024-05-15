namespace weatherstation.Domain.Model
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
        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        public User()
        {
            Notifications = new List<Notification>();
        }

        public User(string firstname, string lastName, string password, string email, bool onNotifications, string preferences, string endpoint, string p256dh, string auth)
        {
            FirstName = firstname;
            LastName = lastName;
            Password = password;
            Email = email;
            OnNotifications = onNotifications;
            Preferences = preferences;
            Endpoint = endpoint;
            P256dh = p256dh;
            Auth = auth;

            Notifications = new List<Notification>();
        }
    }
}
