using AnonimousMailServiceTest.Hubs;
using AnonimousMailServiceTest.Models;
using TableDependency.SqlClient;

namespace AnonimousMailServiceTest.SubscribeTableDependencies
{
    public class SubscribeMessageTableDependency : ISubscribeTableDependency
    {
        SqlTableDependency<Message> tableDependency;
        MailHub mailHub;

        public SubscribeMessageTableDependency(MailHub mailHub)
        {
            this.mailHub = mailHub;
        }

        public void SubscribeTableDependency(string connectionString)
        {
            tableDependency = new SqlTableDependency<Message>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Message> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {   
                mailHub.SendMessage(e.Entity);
            }
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(Message)} SqlTableDependency error: {e.Error.Message}");
        }
    }
}
