using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WEBSockets.Domain.Models;

namespace weatherstation
{
    class Program
    {
        static void Main(string[] args)
        {
            StartListeningForIoTData();

            Console.ReadLine();
        }

        static async void StartListeningForIoTData()
        {
            TcpListener server = null;
            try
            {
                Int32 port = 2228;
                IPAddress localAddr = IPAddress.Parse("20.13.143.114");

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
                            double light = (double)jsonData["light"];

                            Console.WriteLine($"Temperature: {temperature}, Humidity: {humidity}, Light: {light}");

                            string json = JsonConvert.SerializeObject(jsonData);

                            await SendDataToAzureFunction(json);
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

        static async Task SendDataToAzureFunction(string data)
        {
            try
            {
                string azureFunction = "https://weatherstation4dev.azurewebsites.net/api/InsertData";
                HttpClient client = new HttpClient();

                var content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(azureFunction, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data sent to Azure Function successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to send data to Azure Functions.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending data to Azure Functions: " + e.Message);
            }
        }
    }
}
