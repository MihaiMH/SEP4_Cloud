using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;
using weatherstation.EfcDataAccess;

namespace WEBSockets.Core
{
    public class WebSocketHandler
    {
        public static async Task HandleWebSocket(HttpContext context, DatabaseContext dbContext)
        {
            using var socket = await context.WebSockets.AcceptWebSocketAsync();

            Console.WriteLine(" -> A new client connected!");

            string requestRoute = context.Request.Path.ToString();
            string token = context.Request.Query["token"];

            bool connectionAlive = true;
            List<byte> webSocketPayload = new List<byte>(1024 * 4);
            byte[] tempMessage = new byte[1024 * 4];

            while (connectionAlive)
            {
                webSocketPayload.Clear();

                WebSocketReceiveResult? webSocketResponse;

                do
                {
                    webSocketResponse = await socket.ReceiveAsync(tempMessage, CancellationToken.None);
                    webSocketPayload.AddRange(new ArraySegment<byte>(tempMessage, 0, webSocketResponse.Count));
                }
                while (webSocketResponse.EndOfMessage == false);

                if (webSocketResponse.MessageType == WebSocketMessageType.Text)
                {
                    string message = System.Text.Encoding.UTF8.GetString(webSocketPayload.ToArray());
                    Console.WriteLine("Client says {0}", message);

                    try
                    {
                        var jsonData = JsonConvert.DeserializeObject<JObject>(message);

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
                        await dbContext.SaveChangesAsync();
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine("Error parsing JSON: " + ex.Message);
                    }
                }
                else if (webSocketResponse.MessageType == WebSocketMessageType.Close)
                {
                    connectionAlive = false;
                }
            }

            Console.WriteLine(" -> A client disconnected.");
        }
    }
}