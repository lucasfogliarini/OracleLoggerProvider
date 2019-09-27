using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OracleLoggerProvider
{
    internal class OracleLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _connectionString;
        private readonly LogConfiguration _logConfiguration;

        public OracleLogger(string categoryName, string connectionString, LogConfiguration logConfiguration)
        {
            _categoryName = categoryName;
            _connectionString = connectionString;
            _logConfiguration = logConfiguration;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            bool allowLogLevel = logLevel == LogLevel.Critical || logLevel == LogLevel.Error || logLevel == LogLevel.Warning;
            if (allowLogLevel)
            {
                InsertLog(logLevel, eventId, state, exception, formatter);
            }
        }

        internal string GenerateCommandText()
        {
            var sb = new StringBuilder();
            sb.Append("begin ");
            AppendInsertTable(sb);
            AppendColumns(sb);
            AppendValues(sb);
            sb.Append(" end;");
            return sb.ToString();
        }
        private void AppendInsertTable(StringBuilder sb)
        {
            sb.Append($"INSERT INTO {_logConfiguration.TableName}");
        }
        private void AppendColumns(StringBuilder sb)
        {
            sb.Append("(");
            foreach (var column in _logConfiguration.Parameters.Keys)
            {
                sb.Append(column);
                sb.Append(",");
            }
            sb.Length--;
            sb.Append(")");
        }
        private void AppendValues(StringBuilder sb)
        {
            sb.Append("VALUES");
            sb.Append("(");
            foreach (var key in _logConfiguration.Parameters.Keys)
            {
                var paramValue = $":{key}";
                sb.Append(paramValue);
                sb.Append(",");
            }
            sb.Length--;
            sb.Append(");");
        }
        internal void AppendParameters(OracleCommand oracleCommand, Log log)
        {
            foreach (var logParameter in _logConfiguration.Parameters)
            {
                var paramValue = $":{logParameter.Key}";
                var value = log.GetValue(logParameter.Value);
                oracleCommand.Parameters.Add(paramValue, value);
            }
        }
        internal Log GetLogValues<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception)
        {
            var logValues = new Log();
            logValues.SourceContext = _categoryName;
            logValues.LogLevel = logLevel;
            logValues.Date = DateTime.Now;
            logValues.EventId = eventId;
            logValues.State = state;
            logValues.Exception = exception;

            return logValues;
        }
        private void InsertLog<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                
                var command = connection.CreateCommand();
                command.CommandText = GenerateCommandText();
                var logValues = GetLogValues(logLevel, eventId, state, exception);
                AppendParameters(command, logValues);
                command.ExecuteNonQuery();
            }
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return new Disposabler();
        }

        [ExcludeFromCodeCoverage]
        private class Disposabler : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
