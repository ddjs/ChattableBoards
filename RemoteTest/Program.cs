using RemoteShared;
using RemoteShared.DataSets;
using Sockets.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace RemoteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new Client("localhost"))
            {
                client.Message += HandleMessage;

                client.Start();

                while (true)
                {
                    client.Send(new ResponseRequest() { Command = Commands.FindUser, Data = "Justin" });

                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Q)
                    {
                        break;
                    }
                }
            }
        }

        private static void HandleMessage(ITcpSocketClient sender, ResponseRequest e)
        {
            var user = e.Data.ChangeType<User>();
            Console.WriteLine(sender.RemoteAddress + ":" + sender.RemotePort + "\t\tSaid: " + user.ToString());
        }
    }
}
