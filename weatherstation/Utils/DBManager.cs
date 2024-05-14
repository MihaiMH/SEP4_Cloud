using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherstation.Utils
{
    internal class DBManager
    {
        private string? ConnectionString;

        public DBManager(string? connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public List<T> ExecuteQuery<T>(string query, Func<MySqlDataReader, T> map)
        {
            List<T> results = new List<T>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T dto = map(reader);
                            results.Add(dto);
                        }
                    }
                }

                connection.Close();
            }

            return results;
        }
        public void InsertData(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
