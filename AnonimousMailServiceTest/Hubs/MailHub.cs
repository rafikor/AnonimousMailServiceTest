using AnonimousMailServiceTest.Models;
using AnonimousMailServiceTest.SubscribeTableDependencies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace AnonimousMailServiceTest.Hubs
{
    public class MailHub : Hub
    {
        static Dictionary<string, List<string>> connectionsByUser = new Dictionary<string, List<string>>();
        MessageRepository productRepository;

        public MailHub(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AnonimousMailServiceTestContext");
            productRepository = new MessageRepository(connectionString);
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userName = httpContext.Request.Query["userName"][0];
            if(!connectionsByUser.ContainsKey(userName))
            {
                connectionsByUser[userName] = new List<string>();
            }
            connectionsByUser[userName].Add(Context.ConnectionId);
            List<Message> messagesToSend = productRepository.GetMessages(userName);
            var resultToReturn = base.OnConnectedAsync();
            foreach (var message in messagesToSend)
            {
                SendMessage(message);
            }
            return resultToReturn;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userName = httpContext.Request.Query["userName"][0];
            
            connectionsByUser[userName].Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(Message messageToSend)
        {
            var recipient = messageToSend.Recipient;
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
                    await Clients.Client(connectionID).SendAsync("ReceiveMessage", messageToSend.Recipient, messageToSend.Title, messageToSend.Body, messageToSend.TimeSent.ToString("MM/dd/yyyy HH:mm:ss"));
                }
                if (isNeedToUpdateListOfConnections)
                {
                    connectionsByUser[recipient] = copyConnectionIDsOfClient;
                }
            }
        }
    }
}
