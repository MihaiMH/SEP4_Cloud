using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using weatherstation.Domain.Model;
using weatherstation.Utils;



namespace weatherstation.Application.Logic
{
    internal class AccountLogic 
    {

        public AccountLogic()
        {        }

        public static Task LoginAccount(dynamic data)
        {
            string email = data["email"];
            string password = data["password"];

            // Retrieve user from the database
            var someUser = GetUserByEmail(email);

            // Check if the user exists
            if (someUser == null)
            {
                throw new ArgumentException("Invalid email or password."); // Or provide a more specific error message
            }

            // Verify the password
            if (!BCrypt.Net.BCrypt.Verify(password, someUser.Password))
            {
                throw new ArgumentException("Invalid email or password.");
            }

            // Generate JWT token
            string jwtToken = GenerateJwtToken(someUser);

            // Return the JWT token
            return Task.FromResult(jwtToken);
        }


        public static Task RegisterAccount(dynamic data)
        {
            string firstname = data["firstname"];
            string lastname = data["lastname"];
            string password = data["password"];
            string preferences = data["preferences"];
            string email = data["email"];
            bool onNotifications = data["onNotifications"];


            if (password.Length <= 6)
            {
                throw new ArgumentException("Password must be more than 6 characters.");
            }

            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format.");
            }

            if (!IsEmailUnique(email))
            {
                throw new ArgumentException("Email already in use");
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);


            string queryTemplate = Environment.GetEnvironmentVariable("SQLCON1Q3", EnvironmentVariableTarget.Process);


            string query = queryTemplate
                .Replace("[VAR_FIRSTNAME]", firstname)
                .Replace("[VAR_LASTNAME]", lastname)
                .Replace("[VAR_PASSWORD]", hashedPassword)
                .Replace("[VAR_PREFERENCES]", preferences)
                .Replace("[VAR_EMAIL]", email)
                .Replace("[VAR_ONNOTIFICATIONS]", onNotifications ? "true" : "false");



            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            var results = db.ExecuteQuery(query, reader => (object)null);


            return Task.CompletedTask;

        }
        private static bool IsEmailUnique(string email)
        {
            string someQuery = Environment.GetEnvironmentVariable("SQLCON1Q7", EnvironmentVariableTarget.Process);

            string query = someQuery
                  .Replace("{email}", email);

            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            var result = db.ExecuteQuery(query, reader => reader.GetInt32(0));

            return result.FirstOrDefault() == 0;
        }
        private static User GetUserByEmail(string email)
        {
            string someQuery = Environment.GetEnvironmentVariable("SQLCON1Q8", EnvironmentVariableTarget.Process);

          string query = someQuery
                .Replace("{email}", email);


            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            var result = db.ExecuteQuery(query, reader =>
                new User
                {
                    Id = reader.GetInt32("Id"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Email = reader.GetString("Email"),
                    OnNotifications = reader.GetBoolean("OnNotifications"),
                    Preferences = reader.GetString("Preferences")
                });
            User newUser = new User
            {
                Id = result.FirstOrDefault().Id,
                FirstName = result.FirstOrDefault().FirstName,
                LastName = result.FirstOrDefault().LastName,
                Email = result.FirstOrDefault().Email,
                OnNotifications = result.FirstOrDefault().OnNotifications,
                Preferences = result.FirstOrDefault().Preferences
            };


            return newUser;
        }

        private static string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ladn"); // Change this to your secret key
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.Name, user.LastName),
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour, change this as needed
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                            SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private static bool IsValidEmail(string email)
        {

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            Regex regex = new Regex(emailPattern);

            return regex.IsMatch(email);
        }
    }
}
