using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace AnonimousMailServiceTest.Hubs
{
    public class MailHub : Hub
    {
        static Dictionary<string, List<string>> connectionsByUser = new Dictionary<string, List<string>>();

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userName = httpContext.Request.Query["userName"][0];
            if(!connectionsByUser.ContainsKey(userName))
            {
                connectionsByUser[userName] = new List<string>();
            }
            connectionsByUser[userName].Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userName = httpContext.Request.Query["userName"][0];
            
            connectionsByUser[userName].Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string author, string recipient, string theme, string message)
        {
            //TODO: save message to the DB

            if (connectionsByUser.ContainsKey(recipient))
            {
                var connectionIDsOfClient = connectionsByUser[recipient];
                var time = DateTime.UtcNow;
                var copyConnectionIDsOfClient = connectionIDsOfClient;
                bool isNeedToUpdateListOfConnections = false;

                foreach (string connectionID in connectionIDsOfClient)
                {
                    var recipientClient = Clients.Client(connectionID);
                    if (recipientClient == null)
                    {
                        copyConnectionIDsOfClient.Remove(connectionID);
                        isNeedToUpdateListOfConnections = true;
                        continue;
                    }
                    await Clients.Client(connectionID).SendAsync("ReceiveMessage", recipient, theme, message, time.ToString("MM/dd/yyyy HH:mm:ss"));
                }
                if (isNeedToUpdateListOfConnections)
                {
                    connectionsByUser[recipient] = copyConnectionIDsOfClient;
                }
            }
        }
    }
}
