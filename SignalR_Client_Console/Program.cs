using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Client_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new Microsoft.AspNet.SignalR.Client.Connection("http://localhost:33333/Connections/ChatConnection");

            connection.Received +=Console.WriteLine;
            connection.Start().Wait();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                connection.Send(line).Wait();
            }
        }
    }
}
