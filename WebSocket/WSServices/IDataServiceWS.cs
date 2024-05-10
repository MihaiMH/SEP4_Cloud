using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.WSServices
{
    public interface IDataServiceWS
    {
        public Task GetDataAsync(string data);
    }
}
