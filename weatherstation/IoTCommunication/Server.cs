using System.Net;
using System.Net.Sockets;
using System.Text;
using MySql.Data.MySqlClient;

public class Server
{
    static string connectionString = "Server=localhost;Database=weatherstation;Uid=root;Pwd=1234;";
    
    public static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            Int32 port = 12345;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            
            server = new TcpListener(localAddr, port);
            
            server.Start();
            
            Byte[] bytes = new Byte[256];
            String data = null;
            
            while (true)
            {
                Console.Write("Waiting for a connection... ");
                
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                
                data = null;
                
                NetworkStream stream = client.GetStream();
                
                int i;
                
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);
                    
                    ProcessData(data);
                }
                
                client.Close();
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server.Stop();
        }
        
        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }

    static void ProcessData(string data)
    {
        try
        {
            string[] parts = data.Split(',');
            double humidity = double.Parse(parts[0]);
            double temperature = double.Parse(parts[1]);
            string light = parts[2];

            InsertDataIntoDatabase(humidity, temperature, light);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error processing data: {0}", e.Message);
        }
    }
    
    static void InsertDataIntoDatabase(double humidity, double temperature, string light)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                
                string query = "INSERT INTO Weather (Humidity, Temperature, Light, DateTime) VALUES (@Humidity, @Temperature, @Light, @DateTime)";
                
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Humidity", humidity);
                    command.Parameters.AddWithValue("@Temperature", temperature);
                    command.Parameters.AddWithValue("@Light", light);
                    command.Parameters.AddWithValue("@DateTime", DateTime.Now);
                    
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error inserting data into database: {0}", e.Message);
        }
    }
}