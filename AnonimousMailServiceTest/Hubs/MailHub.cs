using AnonimousMailServiceTest.Models;
using AnonimousMailServiceTest.SubscribeTableDependencies;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace AnonimousMailServiceTest.Hubs
{
    public class MailHub : Hub
    {
        static Dictionary<string, List<string>> connectionsByUser = new Dictionary<string, List<string>>();
        MessageRepository messageRepository;

        readonly static string ReceivePossibleRecipientsSignalRProcedureName = "ReceivePossibleRecipients";

        public MailHub(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AnonimousMailServiceTestContext");
            messageRepository = new MessageRepository(connectionString);
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userName = httpContext.Request.Query["userName"][0];
            Groups.AddToGroupAsync(Context.ConnectionId, GetGroupNameByUserName(userName));
            List<Message> messagesToSend = messageRepository.GetMessages(userName);
            var resultToReturn = base.OnConnectedAsync();
            SendMessages(messagesToSend, userName);
            List<string> distinctUsers = messageRepository.GetDistinctUsers();
            SendPossibleRecipients(userName, distinctUsers);
            return resultToReturn;
        }

        public async Task SendMessages(List<Message> messagesToSend, string recipient)
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.DateFormatString = "MM/dd/yyyy HH:mm:ss";
            var jsonToSend = JsonConvert.SerializeObject(messagesToSend, jsonSettings);
            await Clients.Group(GetGroupNameByUserName(recipient)).SendAsync("ReceiveMessages", jsonToSend);
        }

        public async Task SendPossibleRecipients(string recipientToWhomSend, List<string> possibleRecipients)
        {
            var jsonToSend = JsonConvert.SerializeObject(possibleRecipients);
            await Clients.Group(GetGroupNameByUserName(recipientToWhomSend)).SendAsync(ReceivePossibleRecipientsSignalRProcedureName, jsonToSend);
        }
        public async Task SendPossibleRecipientsToAll(List<string> possibleRecipients)
        {
            var jsonToSend = JsonConvert.SerializeObject(possibleRecipients);
            await Clients.All.SendAsync(ReceivePossibleRecipientsSignalRProcedureName, jsonToSend);
        }
        private string GetGroupNameByUserName(string userName)
        {
            return $"user_{userName}";
        }
    }
}
