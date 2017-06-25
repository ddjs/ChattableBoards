

namespace RemoteShared
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Sockets.Plugin.Abstractions;
    using System.Text;
    using System.Threading;
    using System.IO;
    using RemoteShared.DataSets;

    public delegate void RemoteMessage(ITcpSocketClient sender, string e);

    public delegate void RemoteResponseRequest(ITcpSocketClient sender, ResponseRequest e);


    public abstract class RemoteBase : IDisposable
    {
        public const int MaxPacket = 1024;

        public const int ClientPort = 786;

        public const int HeaderLength = 4;

        protected static readonly byte[] exitPacket = new byte[] { 0xff, 0xff, 0xff, 0xE, 0x1, 0x0, 0x2, 0xff, 0xff };

     

        private bool isDisposed;

        private bool running;

        protected RemoteBase(string address, int port)
        {
            this.Address = address;
            this.Port = port;
        }

        public event RemoteMessage ReceivedMessage;

        public event RemoteResponseRequest Message;

        public string Address { get; }

        public abstract string RemoteAddress { get; }

        public int Port { get; }

        public string Name { get => this.GetType().Name; }

        public abstract bool Start();

        public abstract void Stop();

        public abstract void HandleMessage(string message);

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
            if (packet.Equals(exitPacket))
            {
                this.running = false;
                return;
            }

            if (ResponseRequest.IsResponseRequestPacket(packet, out ResponseRequest temp))
            {
                this.Message?.Invoke(client, temp);
            }
            else
            {
                this.ReceivedMessage?.Invoke(client, Encoding.UTF8.GetString(packet, 0, packet.Length));
            }
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
