using System.Data.SqlClient;
using System.Data;
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

        /*public List<Message> GetMessages(string userName)
        {
            List<Message> messages = new List<Message>();
            Message message;

            var data = GetMessagesFromDb(userName);

            foreach (DataRow row in data.Rows)
            {
                message = new Message
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Author = row["Author"].ToString(),
                    Recipient = row["Recipient"].ToString(),
                    Title = row["Title"].ToString(),
                    Body = row["Body"].ToString(),
                    TimeSent = Convert.ToDateTime(row["TimeSent"]),
                };
                messages.Add(message);
            }

            return messages;
        }*/

        public List<Message> GetMessages(string userName)
        {
            var query = $"SELECT Id, Author, Recipient, Title, Body, TimeSent FROM Message WHERE Recipient ={userName} ORDER BY TimeSent";
            List<Message> messages = new List<Message>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
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
