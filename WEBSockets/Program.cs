using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WEBSockets.Domain.Models;
using WEBSockets.EfcDataAccess;

namespace weatherstation
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseContext dbContext = new DatabaseContext();

            // Start listening for IoT device connections
            StartListeningForIoTData(dbContext);

            // Keep the console application running
            Console.ReadLine();
        }

        static void StartListeningForIoTData(DatabaseContext dbContext)
        {
            TcpListener server = null;
            try
            {
                Int32 port = 11000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                server.Start();

                Byte[] bytes = new Byte[1024];
                String data = null;

                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    NetworkStream stream = client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.UTF8.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        try
                        {
                            var jsonData = Newtonsoft.Json.Linq.JObject.Parse(data);

                            double temperature = (double)jsonData["temperature"];
                            double humidity = (double)jsonData["humidity"];
                            int light = (int)jsonData["light"];

                            Console.WriteLine($"Temperature: {temperature}, Humidity: {humidity}, Light: {light}");

                            var weatherData = new WeatherData
                            {
                                WeatherState = "Hz inca",
                                Temperature = temperature,
                                Humidity = humidity,
                                Light = light.ToString(),
                                DateTime = DateTime.UtcNow
                            };

                            dbContext.WeatherData.Add(weatherData);
                            dbContext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error processing data: " + ex.Message);
                        }
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
        }
    }
}
