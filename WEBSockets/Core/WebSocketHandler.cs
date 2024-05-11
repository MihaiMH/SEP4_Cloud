using System.Net.WebSockets;
using System.Text;

namespace WEBSockets.Core
{
    public class WebSocketHandler
    {

        public static async Task HandleWebSocket(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var socket = await context.WebSockets.AcceptWebSocketAsync();
                var buffer = new byte[1024 * 4]; 

                // Ne conectam
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    // Facem handle la incoming messages
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    else
                    {
                        // Procesam incoming data
                        var messageBytes = new byte[result.Count];
                        Array.Copy(buffer, messageBytes, result.Count);
                        string message = Encoding.UTF8.GetString(messageBytes);
                        Console.WriteLine("Received message: " + message);

                        // Trimitem inapoi la ciuspani datele
                        await socket.SendAsync(new ArraySegment<byte>(messageBytes), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    }
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }
}