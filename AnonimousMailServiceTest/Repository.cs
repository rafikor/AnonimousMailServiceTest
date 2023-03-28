using System.Data.SqlClient;
using AnonimousMailServiceTest.Models;
using System.Runtime.Intrinsics.Arm;

namespace AnonimousMailServiceTest
{
    public class MessageRepository
    {
        string connectionString;

        public MessageRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Message> GetMessages(string userName)
        {
            var query = $"SELECT Id, Author, Recipient, Title, Body, TimeSent FROM Message WHERE Recipient =@RecipientName ORDER BY TimeSent";
            List<Message> messages = new List<Message>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.Parameters.Add("@RecipientName", System.Data.SqlDbType.NVarChar);
                        command.Parameters["@RecipientName"].Value = userName;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //Didn't use DataTable.Load(reader) due to strange "Method is not implemented" error
                            while (reader.Read())
                            {
                                var message = new Message();
                                message.Id = Convert.ToInt32(reader["Id"]);
                                message.Author = (string)reader["Author"];
                                message.Recipient = (string)reader["Recipient"];
                                message.Title = (string)reader["Title"];
                                message.Body = (string)reader["Body"];
                                message.TimeSent = Convert.ToDateTime(reader["TimeSent"]);
                                messages.Add(message);
                            }
                        }
                    }

                    return messages;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
