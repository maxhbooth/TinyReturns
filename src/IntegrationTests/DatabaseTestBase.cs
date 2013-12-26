using System;
using System.Data.SqlClient;
using System.Text;
using Dimensional.TinyReturns.Core;

namespace Dimensional.TinyReturns.IntegrationTests
{
    public class DatabaseTestBase : IntegrationTestBase
    {
        protected ISystemLog SystemLog;

        public DatabaseTestBase()
        {
            SystemLog = MasterFactory.SystemLog;
        }

        protected void ConnectionExecuteWithLog(
            string connectionString,
            Action<SqlConnection> connectionAction,
            string logSql)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                SystemLog.Info("Executing: " + logSql);

                sqlConnection.Open();
                connectionAction(sqlConnection);
                sqlConnection.Close();
            }
        }

        protected void ConnectionExecuteWithLog(
            string connectionString,
            Action<SqlConnection> connectionAction,
            string logSql,
            object values)
        {
            var propertyInfos = values.GetType().GetProperties();

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Executing:");
            stringBuilder.AppendLine(logSql);
            stringBuilder.AppendLine("Values:");

            foreach (var info in propertyInfos)
            {
                stringBuilder.Append("@" + info.Name + " = ");
                var value = info.GetValue(values, null);

                if (value == null)
                    stringBuilder.AppendLine("NULL,");
                else
                    stringBuilder.AppendLine(value.ToString());
            }

            SystemLog.Info(stringBuilder.ToString());

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                connectionAction(sqlConnection);
                sqlConnection.Close();
            }
        }
    }
}