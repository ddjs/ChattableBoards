using RemoteShared;
using System;

namespace RemoteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server("localhost", Client.ClientPort);
            server.Start();
            server.ReceivedMessage += (s, e) => Console.WriteLine(e);
            var client = new Client("Localhost");
            client.Start();

            client.Send("Hello");
            client.ReceivedMessage += (s, e) => Console.WriteLine(e);

            Console.ReadKey(true);

            server.Stop();

            while (true) ;
        }
    }
}
