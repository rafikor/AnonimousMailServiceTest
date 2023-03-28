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
            Groups.AddToGroupAsync(Context.ConnectionId, GetGroupNameByUserName(userName));
            List<Message> messagesToSend = productRepository.GetMessages(userName);
            var resultToReturn = base.OnConnectedAsync();
            foreach (var message in messagesToSend)
            {
                SendMessage(message);
            }
            return resultToReturn;
        }

        public async Task SendMessage(Message messageToSend)
        {
            var recipient = messageToSend.Recipient;
            await Clients.Group(GetGroupNameByUserName(recipient)).SendAsync("ReceiveMessage", messageToSend.Recipient, messageToSend.Title, messageToSend.Body, messageToSend.TimeSent.ToString("MM/dd/yyyy HH:mm:ss"));
        }

        private string GetGroupNameByUserName(string userName)
        {
            return $"user_{userName}";
        }
    }
}
