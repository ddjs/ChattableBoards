namespace RemoteShared
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Sockets.Plugin.Abstractions;
    using System.Text;
    using System.Threading;
    using RemoteShared.DataSets;

    public delegate void RemoteMessage(ITcpSocketClient sender, string e);

    public delegate void RemoteResponseRequest(ITcpSocketClient sender, ResponseRequest e);


    public abstract class RemoteBase : IDisposable
    {
        /// <summary>
        /// The Max packet size we can send. 
        /// </summary>
        public const int MaxPacket = 1024;

        /// <summary>
        /// The Port we use for Connection.
        /// </summary>
        public const int ConnectionPort = 786;


        /// <summary>
        /// Our Header Length.
        /// </summary>
        public const int HeaderLength = 4;

        /// <summary>
        /// The Exit Packet. 
        /// </summary>
        protected static readonly byte[] exitPacket = new byte[] { 0xff, 0xff, 0xff, 0xE, 0x1 };

        /// <summary>
        /// Flag to store Disposed or not.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Flag to store our running State.
        /// </summary>
        private bool running;

        /// <summary>
        /// Initialize and Instance of our RemoteBase
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        protected RemoteBase(string address, int port)
        {
            this.Address = address;
            this.Port = port;
        }

        /// <summary>
        /// The string message Event.
        /// </summary>
        public event RemoteMessage StringMessage;

        /// <summary>
        /// The RequestResponse Message event.
        /// </summary>
        public event RemoteResponseRequest Message;

        /// <summary>
        /// Gets the current Address
        /// </summary>
        public string Address { get; }


        /// <summary>
        /// Gets the remote address
        /// </summary>
        public abstract string RemoteAddress { get; }


        /// <summary>
        /// The Port we are connecting with
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// The Name of the instance.
        /// </summary>
        public string Name { get => this.GetType().Name; }

        public abstract bool Start();

        public abstract void Stop();

        protected virtual bool Send(ITcpSocketClient client, byte[] buffer)
        {
            if (!client.WriteStream.CanWrite)
            {
                return false;
            }

            client.WriteStream.Write(buffer, 0, buffer.Length);
            return true;
        }

        protected virtual async Task Reader(ITcpSocketClient client, CancellationToken cancel = default(CancellationToken))
        {
            // get the read stream. 
            var stream = client.ReadStream;
            this.running = true;

            while (stream.CanRead && running)
            {
                // create a buffer to store the data from the client.
                var buffer = new byte[MaxPacket + 1];

                try
                {
                    // Receive the data into the buffer
                    // and store the count of data we have.
                    var count = await stream.ReadAsync(buffer, 0, MaxPacket, cancel);

                    // send the actual bytes to the process method. 
                    this.ProcessPacket(client, buffer.Take(count).ToArray());
                }
                catch
                {
                    // ignore
                    break;
                }
            }
        }

        protected virtual void ProcessPacket(ITcpSocketClient client, byte[] packet)
        {
            if (packet.SequenceEqual(exitPacket))
            {
                // we should exit our reading.
                this.running = false;
                return;
            }

            if (ResponseRequest.IsResponseRequestPacket(packet, out ResponseRequest temp))
            {
                this.Message?.Invoke(client, temp);
                return;
            }

            this.StringMessage?.Invoke(client, Encoding.UTF8.GetString(packet, 0, packet.Length));
        }

        #region IDisposable Support


        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            if (disposing)
            {
                this.Stop();
            }

            isDisposed = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
