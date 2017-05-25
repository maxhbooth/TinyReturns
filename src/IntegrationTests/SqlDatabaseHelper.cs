using System;
using System.Data.SqlClient;
using System.Text;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.SharedContext.Services;

namespace Dimensional.TinyReturns.IntegrationTests
{
    public class SqlDatabaseHelper
    {
        public static void ConnectionExecuteWithLog(
            string connectionString,
            Action<SqlConnection> connectionAction,
            string logSql)
        {
            var systemLog = CreateSystemLog();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                systemLog.Info("Executing: " + logSql);

                sqlConnection.Open();
                connectionAction(sqlConnection);
                sqlConnection.Close();
            }
        }

        public static void ConnectionExecuteWithLog(
            string connectionString,
            Action<SqlConnection> connectionAction,
            string logSql,
            object values)
        {
            var systemLog = CreateSystemLog();

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

            systemLog.Info(stringBuilder.ToString());

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                connectionAction(sqlConnection);
                sqlConnection.Close();
            }
        }

        private static ISystemLog CreateSystemLog()
        {
            return new SystemLogForIntegrationTests();
        }
    }
}