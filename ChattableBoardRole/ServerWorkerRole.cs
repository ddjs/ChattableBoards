using System.Diagnostics;
using System.Net;
using Microsoft.WindowsAzure.ServiceRuntime;
using RemoteShared;
using RemoteShared.DataSets;

namespace ChattableBoardRole
{
    public class ServerWorkerRole : RoleEntryPoint
    {
        private Server server;

        public override void Run()
        {
            Trace.TraceInformation("ChattableBoardRole is running");
            var endPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["Listener"].IPEndpoint;

            this.server = new Server(endPoint.Address.ToString(), endPoint.Port);
            server.Message += this.OnRequest;
            server.Start();

            while (true) ;
        }

        private void OnRequest(Sockets.Plugin.Abstractions.ITcpSocketClient sender, ResponseRequest e)
        {
            var response = new ResponseRequest { Command = e.Command, Status = MessageStatus.Success, Data = 123 };

            switch (e.Command)
            {
                case Commands.Login:
                    break;

                case Commands.CreateUser:
                    break;

                case Commands.JoinRoom:
                    break;

                case Commands.ListRooms:
                    break;

                case Commands.CreateRoom:
                    break;

                case Commands.InviteUser:
                    break;

                case Commands.FindUser:
                    response.Data = new User { Name = "Justin", Id = 12424324 };
                    break;

                default:
                    response.Status = MessageStatus.Fail;
                    break;
            }

            this.server.NotifyClient(sender, response);
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("ChattableBoardRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ChattableBoardRole is stopping");
            if (server.IsRunning)
            {
                this.server.Dispose();
            }

            base.OnStop();
            Trace.TraceInformation("ChattableBoardRole has stopped");
        }
    }
}
