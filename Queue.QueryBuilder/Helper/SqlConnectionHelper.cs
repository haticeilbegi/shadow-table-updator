using Microsoft.Data.SqlClient;
using Queue.QueryBuilder.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.QueryBuilder.Helper
{
    public interface ISqlConnectionHelper
    {
        SqlCommand CreateCommand(Database database);
    }

    public class SqlConnectionHelper : ISqlConnectionHelper
    {
        public SqlCommand CreateCommand(Database database)
        {
            SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = database.Server,
                UserID = database.User,
                Password = database.Password,
                InitialCatalog = database.Name
            };

            return new SqlCommand
            {
                Connection = new SqlConnection(connectionBuilder.ConnectionString)
            };
        }
    }
}
