using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Gateway
{
    public interface IWebSocketClient
    {
        public Task ConnectAsync();
        public Task DisconnectAsync();
        public Task ReceiveAsync();
        public Task SendAsync();
    }
}
