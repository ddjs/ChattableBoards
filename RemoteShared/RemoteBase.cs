

namespace RemoteShared
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Sockets.Plugin.Abstractions;
    using System.Text;

    public abstract class RemoteBase
    {
        public const int MaxPacket = 1024;

        protected RemoteBase(string address, int port)
        {
            this.Address = address;
            this.Port = port;
        }

        public event EventHandler<string> ReceivedMessage;

        public string Address { get; }

        public int Port { get; }

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

        protected virtual async Task Reader(ITcpSocketClient client)
        {
            // get the read stream. 
            var stream = client.ReadStream;

            while (stream.CanRead)
            {
                // create a buffer to store the data from the client.
                var buffer = new byte[MaxPacket + 1];

                // Receive the data into the buffer
                // and store the count of data we have.
                var count = await stream.ReadAsync(buffer, 0, MaxPacket);

                // send the actual bytes to the process method. 
                this.ProcessPacket(client, buffer.Take(count).ToArray());
            }
        }

        protected virtual void ProcessPacket(ITcpSocketClient client, byte[] packet)
        {
            Debug.WriteLine(client.RemoteAddress + " Sends: " + BitConverter.ToString(packet));
            this.ReceivedMessage?.Invoke(this, Encoding.UTF8.GetString(packet, 0, packet.Length));
        }
    }
}
