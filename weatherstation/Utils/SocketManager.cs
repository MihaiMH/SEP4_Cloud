using System.Net.Sockets;
using System.Text;

namespace weatherstation.Utils
{
    public class SocketManager
    {
        private readonly string _serverAddress;
        private readonly int _serverPort;

        public SocketManager(string serverAddress, int serverPort)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
        }

        public async Task<string> SendMessageAndWaitForResponseAsync(string message)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(_serverAddress, _serverPort);
                    using (var networkStream = client.GetStream())
                    {
                        // Send message to the server
                        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                        await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length);

                        // Wait for the server response
                        byte[] buffer = new byte[1024];
                        int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        string jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        return jsonResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (logging, rethrowing, etc.)
                throw new InvalidOperationException("An error occurred while communicating with the socket server.", ex);
            }
        }
    }
}
