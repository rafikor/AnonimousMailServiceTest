using AnonimousMailServiceTest.Models;
using AnonimousMailServiceTest.Repositories;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

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
            var isOnlySendNewMessages = httpContext.Request.Query["onlyNew"];
            var resultToReturn = base.OnConnectedAsync();
            var userName = httpContext.Request.Query["userName"][0];
            if (!isOnlySendNewMessages.Any())
            {
                Groups.AddToGroupAsync(Context.ConnectionId, GetGroupNameByUserName(userName));
                List<Message> messagesToSend = messageRepository.GetMessages(userName);
                
                SendMessages(messagesToSend, userName, isPopups:false);
                List<string> distinctUsers = messageRepository.GetDistinctUsers();
                SendPossibleRecipients(userName, distinctUsers);
            }
            else
            {
                Groups.AddToGroupAsync(Context.ConnectionId, GetGroupOnlyNotifyNameByUserName(userName));
            }
            return resultToReturn;
        }

        public async Task SendMessages(List<Message> messagesToSend, string recipient, bool isPopups = true)
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.DateFormatString = "MM/dd/yyyy HH:mm:ss";
            var jsonToSend = JsonConvert.SerializeObject(messagesToSend, jsonSettings);
            await Clients.Group(GetGroupNameByUserName(recipient)).SendAsync("ReceiveMessages", jsonToSend);
            if (isPopups)
            {
                await Clients.Group(GetGroupOnlyNotifyNameByUserName(recipient)).SendAsync("ReceiveMessagesPopup", jsonToSend);
            }
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
        private string GetGroupOnlyNotifyNameByUserName(string userName)
        {
            return $"newOnly_{userName}";
        }
    }
}
