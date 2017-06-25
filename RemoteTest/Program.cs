using RemoteShared;
using Sockets.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<IDisposable>();

            using (var server = new Server("localhost", Client.ClientPort))
            {
                server.Start();
                server.ReceivedMessage += HandleMessage;
             
                for (int i = 0; i < 10; i++)
                {
                    var client = new Client("Localhost");

                    client.ReceivedMessage += HandleMessage;

                    client.Start();

                    client.Send("Hello");
                    list.Add(client);

                }

                Console.ReadKey(true);

                server.NotifyClients("Bye");


            }

            list.ForEach(x => x.Dispose());
            
            while (true)
            {

            }
        }

        private static void HandleMessage(ITcpSocketClient sender, string e)
        {
            Console.WriteLine(sender.RemoteAddress + ":" + sender.RemotePort +  "\t\tSaid: " + e);
        }
    }
}
