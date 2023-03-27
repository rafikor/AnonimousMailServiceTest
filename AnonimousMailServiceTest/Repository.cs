using System.Data.SqlClient;
using System.Data;
using AnonimousMailServiceTest.Models;

namespace AnonimousMailServiceTest
{
    public class MessageRepository
    {
        string connectionString;

        public MessageRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Message> GetProducts()
        {
            List<Message> products = new List<Message>();
            Message product;

            var data = GetProductDetailsFromDb();

            foreach (DataRow row in data.Rows)
            {
                product = new Message
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Author = row["Author"].ToString(),
                    Recipient = row["Recipient"].ToString(),
                    Title = row["Title"].ToString(),
                    Body = row["Body"].ToString(),
                    TimeSent = Convert.ToDateTime(row["TimeSent"]),
                };
                products.Add(product);
            }

            return products;
        }

        private DataTable GetProductDetailsFromDb()
        {
            var query = "SELECT Id, Author, Recipient, Title, Body, TimeSent FROM Message";
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dataTable.Load(reader);
                        }
                    }

                    return dataTable;
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
