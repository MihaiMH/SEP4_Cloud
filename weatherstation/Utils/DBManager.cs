﻿using MySql.Data.MySqlClient;

namespace weatherstation.Utils
{
    public class DBManager
    {
        private string? ConnectionString;

        public DBManager(string? connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public async Task<List<T>> ExecuteQuery<T>(string query, Func<MySqlDataReader, Task<T>> map)
        {
            List<T> results = new List<T>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            T dto = await map(reader);
                            results.Add(dto);
                        }
                    }
                }

                await connection.CloseAsync();
            }

            return results;
        }
        public async Task InsertData(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                await connection.CloseAsync();
            }
        }
    }
}
