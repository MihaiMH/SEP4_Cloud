using MySqlX.XDevAPI;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace weatherstation
{
    class Program
    {
        static ConcurrentDictionary<Guid, TcpClient> clients = new ConcurrentDictionary<Guid, TcpClient>();
        static async Task Main(string[] args)
        {
            await StartListeningForIoTData();

            Console.ReadLine();
        }

        static async Task StartListeningForIoTData()
        {
            TcpListener server = null;
            try
            {
                Int32 port = 2228;
                server = new TcpListener(IPAddress.Any, port);

                server.Start();
                Console.WriteLine("Server started. Waiting for connections...");

                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    Guid clientId = Guid.NewGuid();
                    clients.TryAdd(clientId, client);
                    Console.WriteLine("Connected!");

                    _ = Task.Run(() => ProcessClientAsync(client, clientId));
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

        static async Task ProcessClientAsync(TcpClient client, Guid clientId)
        {
            using (client)
            {
                NetworkStream stream = client.GetStream();
                byte[] bytes = new byte[1024];
                StringBuilder data = new StringBuilder();

                try
                {
                    while (true)
                    {
                        int i = await stream.ReadAsync(bytes, 0, bytes.Length);

                        if (i == 0)
                        {
                            Console.WriteLine("Client disconnected.");
                            clients.TryRemove(clientId, out _);
                            break;
                        }

                        data.Append(Encoding.UTF8.GetString(bytes, 0, i));
                        Console.WriteLine("Received: {0}", data);

                        try
                        {
                            var jsonData = Newtonsoft.Json.Linq.JObject.Parse(data.ToString());

                            if (jsonData.TryGetValue("msg", out var msgToken))
                            {
                                string msg = msgToken.ToString();
                                if (msg.Equals("updateWeather"))
                                {
                                    Console.WriteLine("Trigger IoT Device");
                                    await SendMessageToAllClients("updateWeather");
                                }
                            }

                            if (jsonData.TryGetValue("temperature", out var temperatureToken) &&
                                jsonData.TryGetValue("humidity", out var humidityToken) &&
                                jsonData.TryGetValue("light", out var lightToken))
                            {
                                double temperature = (double)temperatureToken;
                                double humidity = (double)humidityToken;
                                double light = (double)lightToken;

                                string json = JsonConvert.SerializeObject(jsonData);

                                Console.WriteLine(json);
                                string res = await SendDataToAzureFunction(json);
                            }

                            data.Clear();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error processing data: " + ex.Message);
                        }
                    }
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine("Error reading JSON data: " + ex.Message);
                }
                catch (IOException ex) when (ex.InnerException is SocketException socketException && socketException.SocketErrorCode == SocketError.ConnectionReset)
                {
                    Console.WriteLine("Client forcibly closed the connection.");
                    clients.TryRemove(clientId, out _);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading from client: " + ex.Message);
                }
            }
        }

        static async Task SendMessageToAllClients(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            foreach (var kvp in clients)
            {
                var clientId = kvp.Key;
                var client = kvp.Value;

                try
                {
                    if (client.Connected)
                    {
                        NetworkStream stream = client.GetStream();
                        await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
                        Console.WriteLine("Message sent to client " + clientId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending message to client " + clientId + ": " + ex.Message);
                }
            }
        }

        static async Task<string> SendDataToAzureFunction(string data)
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

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending data to Azure Functions: " + e.Message);
                return HttpStatusCode.InternalServerError.ToString();
            }
        }
    }
}
