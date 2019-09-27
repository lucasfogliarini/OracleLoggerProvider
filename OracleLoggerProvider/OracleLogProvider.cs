using Microsoft.Extensions.Logging;
using System;

namespace OracleLoggerProvider
{
    public class OracleLogProvider : ILoggerProvider
    {
        private readonly string _connectionString;
        private readonly LogConfiguration _logConfiguration;
        internal const string connectionStringParam = "connectionString";
        internal const string logConfigurationParam = "logConfiguration";
        internal const string logConfigurationParametersParam = "LogConfiguration.Parameters";
        internal const string tableNameParam = "tableName";

        public OracleLogProvider(string connectionString, LogConfiguration logConfiguration)
        {
            CheckIfNull(connectionString, connectionStringParam);
            CheckLogConfiguration(logConfiguration);

            _connectionString = connectionString;
            _logConfiguration = logConfiguration;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new OracleLogger(categoryName, _connectionString, _logConfiguration);
        }

        public void Dispose()
        {
        }

        private void CheckLogConfiguration(LogConfiguration logConfiguration)
        {
            CheckIfNull(logConfiguration, logConfigurationParam);
            CheckIfNull(logConfiguration.TableName, tableNameParam);
            if (logConfiguration.Parameters.Count == 0)
            {
                throw new ArgumentOutOfRangeException(logConfigurationParametersParam);
            }
        }

        private void CheckIfNull(object value, string paramName = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
