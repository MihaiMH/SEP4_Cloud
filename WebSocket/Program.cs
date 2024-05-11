using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket.Gateway;


namespace WebSocket
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Start();
        }

        private static async Task Start()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to WebSocket:" + ex);
            }
            finally
            {
                
            }
        }

    }
}
