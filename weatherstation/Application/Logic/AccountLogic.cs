using System;
using System.Threading.Tasks;
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

        public Task RegisterAccount(User user)
        {
            string query = $"INSERT INTO Users (Username, Password, Email, OnNotifications) " +
                           $"VALUES ('Johnson', '123', 'johnson@gmail.com', 'true')";

            try
            {
              
                DBManager db = new DBManager(connectionString);

                // Execute the command to insert the user into the database
                // Since we're executing an INSERT query, we'll use ExecuteQuery with a dummy mapper
                // Dummy mapper returns null because we don't need to map anything when inserting
                // We only need to know if the insertion was successful or not
                var results = db.ExecuteQuery(query, reader => (object)null);

                // If the execution was successful (no exceptions), return a completed task
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred during account registration: {ex.Message}");
                throw; 
            }
        }
    }
}
