﻿using System;
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



        private static async Task<User?> GetUserByEmail(string email)
        {
            string someQuery = Environment.GetEnvironmentVariable("SQLCON1Q8", EnvironmentVariableTarget.Process);
            string query = someQuery.Replace("{email}", email); // Replace placeholder with actual email value

            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            var result = await db.ExecuteQuery(query, async (reader) =>
            {
                // Check if the reader has rows before attempting to read data
                if (reader.HasRows)
                {
                    // Read the first row
                    await reader.ReadAsync();

                    // Create a new User object
                    return new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")), // Get column index by name
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        OnNotifications = reader.GetBoolean(reader.GetOrdinal("OnNotifications")),
                        Preferences = reader.GetString(reader.GetOrdinal("Preferences"))
                    };
                }
                else
                {
                    // No user found with the given email
                    return null;
                }
            });

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

        public static async Task<string> UpdateAccount(dynamic data)
        {
            string email = data["email"];
            string firstname = data["firstname"];
            string lastname = data["lastname"];
            string password = data["password"];
            string preferences = data["preferences"];
            bool onNotifications = data["onNotifications"];

            var existingUser = await GetUserByEmail(email);

            if (existingUser == null)
            {
                throw new ArgumentException("User not found.");
            }

            if (!string.IsNullOrEmpty(password) && password.Length <= 6)
            {
                throw new ArgumentException("Password must be more than 6 characters.");
            }

            IsValidEmail(email);
            await IsEmailUnique(email);

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

            string query = queryTemplate
                .Replace("[VAR_USERID]", existingUser.Id.ToString())
                .Replace("[VAR_FIRSTNAME]", existingUser.FirstName)
                .Replace("[VAR_LASTNAME]", existingUser.LastName)
                .Replace("[VAR_PASSWORD]", existingUser.Password)
                .Replace("[VAR_PREFERENCES]", existingUser.Preferences)
                .Replace("[VAR_EMAIL]", existingUser.Email)
                .Replace("[VAR_ONNOTIFICATIONS]", existingUser.OnNotifications ? "true" : "false");

            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            await db.InsertData(query);

            // Generate new JWT token for the updated user
            string newJwtToken = GenerateJwtToken(existingUser);

            // Return the new JWT token
            return newJwtToken;
        }


    }
}
