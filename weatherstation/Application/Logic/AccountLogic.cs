using System;
using System.Threading.Tasks;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;
using weatherstation.Application.LogicInterfaces;
using weatherstation.Domain.DTOs;
using weatherstation.Domain.Model;
using weatherstation.Utils;

namespace weatherstation.Application.Logic
{
    internal class AccountLogic : IAccountService
    {
        private readonly string connectionString;

        public AccountLogic(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Task RegisterAccount(dynamic data)
        {
            string username = data["username"];
            string password = data["password"];
            string email = data["email"];
            bool onNotifications = data["onNotifications"];


            if (password.Length <= 6)
            {
                throw new ArgumentException("Password must be more than 6 characters.");
            }

            if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Email must end with '@gmail.com'.");
            }

            if (!IsUsernameUnique(username))
            {
                throw new ArgumentException("Username is already taken.");
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);


            string queryTemplate = Environment.GetEnvironmentVariable("SQLCON1Q3", EnvironmentVariableTarget.Process);


            string query = queryTemplate
                .Replace("[VAR_USERNAME]", username)
                .Replace("[VAR_PASSWORD]", hashedPassword)
                .Replace("[VAR_EMAIL]", email)
                .Replace("[VAR_ONNOTIFICATIONS]", onNotifications ? "true" : "false");



            DBManager db = new DBManager(connectionString);
            var results = db.ExecuteQuery(query, reader => (object)null);


            return Task.CompletedTask;

        }
        private bool IsUsernameUnique(string username)
        {
            string query = $"SELECT COUNT(*) FROM Users WHERE Username = '{username}'";

            DBManager db = new DBManager(connectionString);
            var result = db.ExecuteQuery(query, reader => reader.GetInt32(0));

            return result.FirstOrDefault() == 0;
        }
    }
}