namespace RemoteShared
{
    using RemoteShared.DataSets;
    using Sockets.Plugin;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public class Client : RemoteBase
    {
        /// <summary>
        /// The socket we use to connect to the server.
        /// </summary>
        private readonly TcpSocketClient socket = new TcpSocketClient();

        /// <summary>
        /// The cancellation token for reading from the server.
        /// </summary>
        private readonly CancellationTokenSource cancel = new CancellationTokenSource();


        /// <summary>
        /// The reading Task. 
        /// </summary>
        private Task readingTask;

        /// <summary>
        /// Create an Instance of a Client
        /// </summary>
        /// <param name="ipAddress"> The Address of the server we wish to connect to</param>
        /// <param name="port">The Port of the server. </param>
        public Client(string ipAddress, int port = Port) : base(ipAddress, port)
        {
        }

        /// <summary>
        /// Lets us know if we are connected. 
        /// </summary>
        public bool Connected
        {
            get
            {
                return this.socket.ReadStream.CanRead && this.socket.WriteStream.CanWrite;
            }
        }

        /// <summary>
        /// Gets our remote Address.
        /// </summary>
        public override string RemoteAddress { get => this.socket.RemoteAddress; }

        /// <summary>
        /// Starts the Connection to the server. 
        /// </summary>
        /// <returns>
        /// Successful connection returns true;
        /// </returns>
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

        /// <summary>
        /// Stops the connection to the server.
        /// </summary>
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

        /// <summary>
        /// Sends a Request to the server
        /// </summary>
        /// <param name="message">
        /// The Request to make. 
        /// </param>
        public void Send(ResponseRequest message)
        {
            base.Send(this.socket, message.ToByteArray());
        }

        /// <summary>
        /// Sends a string message to the server
        /// </summary>
        /// <param name="message">
        /// the string to send
        /// </param>
        public void Send(string message)
        {
            base.Send(this.socket, message.ToByteArray());
        }
    }
}
