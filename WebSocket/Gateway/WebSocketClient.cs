using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket.Utils;

namespace WebSocket.Gateway
{
    public class WebSocketClient : IWebSocketClient
    {
        private readonly Uri _uri;
        private ClientWebSocket _socket;
        private DataConvertor dataConvertor;

        public WebSocketClient(string uri)
        {
            _uri = new Uri(uri);
        }

        public async Task ConnectAsync()
        {
            _socket = new ClientWebSocket();

            await _socket.ConnectAsync(_uri, CancellationToken.None);

            Console.WriteLine($"WebSocket connected to {_uri}");
        }

        public async Task DisconnectAsync()
        {
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        public Task ReceiveAsync()
        {
            throw new NotImplementedException();
        }

        public Task SendAsync()
        {
            throw new NotImplementedException();
        }
    }
}
