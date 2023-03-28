using AnonimousMailServiceTest.Hubs;
using AnonimousMailServiceTest.Models;
using TableDependency.SqlClient;

namespace AnonimousMailServiceTest.SubscribeTableDependencies
{
    public class SubscribeUniqueUserTableDependency : ISubscribeTableDependency
    {
        SqlTableDependency<UserOfMailService> tableDependency;
        MailHub mailHub;

        public SubscribeUniqueUserTableDependency(MailHub mailHub)
        {
            this.mailHub = mailHub;
        }

        public void SubscribeTableDependency(string connectionString)
        {
            tableDependency = new SqlTableDependency<UserOfMailService>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<UserOfMailService> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                mailHub.SendPossibleRecipientsToAll(new List<string>() { e.Entity.Name });
            }
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"SubscribeUniqueUserTableDependency SqlTableDependency error: {e.Error.Message}");
        }
    }
}
