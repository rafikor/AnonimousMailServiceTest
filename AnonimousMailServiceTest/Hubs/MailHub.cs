using AnonimousMailServiceTest.Models;
using AnonimousMailServiceTest.SubscribeTableDependencies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Newtonsoft.Json;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

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
            SendMessages(messagesToSend, userName);
            return resultToReturn;
        }

        public async Task SendMessages(List<Message> messagesToSend, string recipient)
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.DateFormatString = "MM/dd/yyyy HH:mm:ss";
            var jsonToSend = JsonConvert.SerializeObject(messagesToSend, jsonSettings);
            await Clients.Group(GetGroupNameByUserName(recipient)).SendAsync("ReceiveMessages", jsonToSend);
        }

        private string GetGroupNameByUserName(string userName)
        {
            return $"user_{userName}";
        }
    }
}
