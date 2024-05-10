using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Utils
{
    public class DataConvertor
    {
        public string GetData(string iotData)
        {
            var jObject = JObject.Parse(iotData);

            if (jObject["data"] == null)
            {
                throw new InvalidDataException();
            }

            return jObject["data"].Value<string>();
        }
    }
}
