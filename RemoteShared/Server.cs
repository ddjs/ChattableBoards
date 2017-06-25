namespace RemoteShared
{
    using Sockets.Plugin;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Sockets.Plugin.Abstractions;
    using System.Threading;

    public class Server : RemoteBase
    {
        // the server instance. 
        private readonly TcpSocketListener server = new TcpSocketListener();

        // the list of clients.
        private readonly List<ITcpSocketClient> clients = new List<ITcpSocketClient>(100);

        // flag to not allow two servers. 
        private bool running;

        public Server(string address, int port) : base(address, port)
        {

        }

        public override string RemoteAddress { get => "Server"; }

        public List<ITcpSocketClient> Clients
        {
            get
            {
                return this.clients;
            }
        }

        public bool IsRunning
        {
            get
            {
                return this.running;
            }
        }

        public override bool Start()
        {
            if (this.running)
            {
                return true;
            }

            // set our running flag to true
            running = true;

            // start the server.
            this.server.StartListeningAsync(this.Port);

            // listen for clients. 
            this.server.ConnectionReceived += async (s, e) => await this.Reader(e.SocketClient, CancellationToken.None);

            return true;
        }

        public override void Stop()
        {
            this.server.StopListeningAsync().Wait();
            this.server.Dispose();
        }


        public void NotifyClients(string message)
        {
            var buffer = message.ToByteArray();

            foreach (var client in this.clients.Where(x => x.WriteStream.CanWrite))
            {
                this.Send(client, buffer);
            }
        }

        public void NotifyClient(ITcpSocketClient client, string message)
        {
            this.Send(client, message.ToByteArray());
        }

        protected override async Task Reader(ITcpSocketClient client, CancellationToken token)
        {
            lock (this.clients)
            {
                // add the client to our list.
                this.clients.Add(client);
            }

            await base.Reader(client, token);

            lock (this.clients)
            {
                // remote the client from our list. 
                this.clients.Remove(client);
            }
        }
    }
}
