﻿namespace RemoteShared
{
    using RemoteShared.DataSets;
    using Sockets.Plugin;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public class Client : RemoteBase
    {


        private readonly TcpSocketClient socket = new TcpSocketClient();

        private readonly CancellationTokenSource cancel = new CancellationTokenSource();

        private Task readingTask;

        public Client(string ipAddress, int port = ClientPort) : base(ipAddress, port)
        {
        }

        public bool Connected
        {
            get
            {
                return this.socket.ReadStream.CanRead && this.socket.WriteStream.CanWrite;
            }
        }

        public override string RemoteAddress { get => this.socket.RemoteAddress; }

        public override bool Start()
        {
            // await the connection.
            this.socket.ConnectAsync(this.Address, this.Port).Wait();

            if (this.Connected)
            {
                // Start the reading Thread.
                this.readingTask = this.Reader(this.socket, cancel.Token);

                return true;
            }

            return false;
        }

        public override void HandleMessage(string message)
        {

        }

        public override void Stop()
        {
            // notify server we are exiting. 
            this.Send(this.socket, exitPacket);

            // cancel our reading Task by use of the cancellation token.
            cancel.Cancel(false);

            // wait for the Task to end. 
            while (this.readingTask.Status == TaskStatus.Running)
            {

            }

            // dispose of the client. 
            this.socket.Dispose();
        }

        public void Send(ResponseRequest message)
        {
            base.Send(this.socket, message.ToByteArray());
        }

        public void Send(string message)
        {
            base.Send(this.socket, message.ToByteArray());
        }
    }
}
