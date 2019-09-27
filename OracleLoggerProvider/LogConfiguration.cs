using System.Collections.Generic;

namespace OracleLoggerProvider
{
    public class LogConfiguration
    {
        internal IDictionary<string, string> Parameters { get; private set; }
        public LogConfiguration(string tableName)
        {
            TableName = tableName;
            Parameters = new Dictionary<string, string>();
        }
        public string TableName { get; private set; }

        public void Add(string columnName, string value)
        {
            Parameters.Add(columnName, value);
        }
        public void Add(string columnName, LogValue logValue)
        {
            var value = Log.GetTemplate(logValue);
            Add(columnName, value);
        }
    }
}
