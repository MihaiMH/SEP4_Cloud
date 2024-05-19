using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using weatherstation.Domain.DTOs;
using weatherstation.Domain.Model;
using weatherstation.Utils;



namespace weatherstation.Application.Logic
{
    public class AccountLogic 
    {

        public AccountLogic()
        {        }

        public static async Task<string> LoginAccount(dynamic data)
        {
            string email = data["email"];
            string password = data["password"];

            // Retrieve user from the database
            var someUser = await GetUserByEmail(email);

            // Check if the user exists
            if (someUser == null)
            {
                throw new ArgumentException("Invalid email or password.");
            }

            // Verify the password
            if (!BCrypt.Net.BCrypt.Verify(password, someUser.Password))
            {
                throw new ArgumentException("Invalid email or password.");
            }

            // Generate JWT token
            string jwtToken = GenerateJwtToken(someUser);

            // Return the JWT token
            return jwtToken;
        }


        public static async Task RegisterAccount(dynamic data)
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

            if (!await IsEmailUnique(email))
            {
                throw new ArgumentException("Email already in use");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);


            string queryTemplate = Environment.GetEnvironmentVariable("SQLCON1Q3", EnvironmentVariableTarget.Process);


            string query = queryTemplate
                .Replace("[VAR_FIRSTNAME]", firstname)
                .Replace("[VAR_LASTNAME]", lastname)
                .Replace("[VAR_PASSWORD]", hashedPassword)
                .Replace("[VAR_PREFERENCES]", preferences)
                .Replace("[VAR_EMAIL]", email)
                .Replace("[VAR_ONNOTIFICATIONS]", onNotifications ? "true" : "false");



            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            await db.InsertData(query);
        }

        private static async Task<bool> IsEmailUnique(string email)
        {
            string someQuery = Environment.GetEnvironmentVariable("SQLCON1Q7", EnvironmentVariableTarget.Process);
            string query = someQuery.Replace("[VAR_EMAIL]", email);

            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            var result = await db.ExecuteQuery(query, async (reader) => await Task.FromResult(reader.GetInt32(0)));
            return result.FirstOrDefault() == 0;
        }



        public static async Task<User?> GetUserByEmail(string email)
        {
            string someQuery = Environment.GetEnvironmentVariable("SQLCON1Q8", EnvironmentVariableTarget.Process);
            string query = someQuery.Replace("[VAR_EMAIL]", email); 

            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));

            var result = await db.ExecuteQuery(
                query, 
                async (reader) => await Task.FromResult(new User
                {
                    Id = reader.GetInt32("Id"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Email = reader.GetString("Email"),
                    Password = reader.GetString("Password"),
                    OnNotifications = reader.GetBoolean("OnNotifications"),
                    Preferences = reader.GetString("Preferences")
                }));

            return result.FirstOrDefault();
        }

        private static string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your_secret_key_here_123456789012345678901234567890"); // Change this to your secret key
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour, change this as needed
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
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

        public static async Task<string> UpdateAccount(dynamic data, Dictionary<string, string> token)
        {
            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            string emailFromToken = token["email"];

            string email = data["email"];
            string firstname = data["firstname"];
            string lastname = data["lastname"];
            string password = data["password"];
            string preferences = data["preferences"];
            bool onNotifications = data["onNotifications"];

            var existingUser = await GetUserByEmail(emailFromToken);

            if (existingUser == null)
            {
                throw new ArgumentException("User not found.");
            }

            if (!string.IsNullOrEmpty(password) && password.Length <= 6)
            {
                throw new ArgumentException("Password must be more than 6 characters.");
            }

            if (!string.IsNullOrEmpty(email))
            {
                if (IsValidEmail(email))
                {
                    if (await IsEmailUnique(email))
                    {
                        existingUser.Email = email;
                    }
                    else
                    {
                        throw new ArgumentException("Email already in use.");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid email format.");
                }
            }
            else
            {
                existingUser.Email = emailFromToken;
            }

            existingUser.FirstName = firstname;
            existingUser.LastName = lastname;
            existingUser.Preferences = preferences;
            existingUser.OnNotifications = onNotifications;

            if (!string.IsNullOrEmpty(password))
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
                existingUser.Password = hashedPassword;
            }

            string queryTemplate = Environment.GetEnvironmentVariable("SQLCON1Q9", EnvironmentVariableTarget.Process);

            queryTemplate = queryTemplate
                .Replace("[VAR_USERID]", existingUser.Id.ToString())
                .Replace("[VAR_FIRSTNAME]", existingUser.FirstName)
                .Replace("[VAR_LASTNAME]", existingUser.LastName)
                .Replace("[VAR_PASSWORD]", existingUser.Password)
                .Replace("[VAR_PREFERENCES]", existingUser.Preferences)
                .Replace("[VAR_EMAIL]", existingUser.Email)
                .Replace("[VAR_ONNOTIFICATIONS]", existingUser.OnNotifications ? "true" : "false");

            await db.InsertData(queryTemplate);

            string newJwtToken = GenerateJwtToken(existingUser);

            return newJwtToken;
        }
    }
}
