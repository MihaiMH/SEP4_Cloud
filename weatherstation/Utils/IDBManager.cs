using MySql.Data.MySqlClient;

namespace weatherstation.Utils
{
    public interface IDBManager
    {
        Task<List<T>> ExecuteQuery<T>(string query, Func<MySqlDataReader, Task<T>> map);
        Task InsertData(string query);
    }
}
