using Microsoft.Extensions.Logging;
using System;

namespace OracleLogging
{
    internal class Log
    {
        public string SourceContext { get; internal set; }
        public LogLevel LogLevel { get; internal set; }
        public DateTime Date { get; internal set; }
        public Exception Exception { get; internal set; }
        public object State { get; internal set; }
        public EventId EventId { get; internal set; }
        public object GetValue(string templateValue)
        {
            if (templateValue == GetTemplate(LogValue.SourceContext))
                return SourceContext;
            if (templateValue == GetTemplate(LogValue.LogLevel))
                return LogLevel.ToString();
            if (templateValue == GetTemplate(LogValue.Exception))
                return Exception.ToString();
            if (templateValue == GetTemplate(LogValue.State))
                return State.ToString();
            if (templateValue == GetTemplate(LogValue.Date))
                return Date;
            if (templateValue == GetTemplate(LogValue.EventId))
                return EventId;
            return templateValue;
        }

        public static string GetTemplate(LogValue logValue)
        {
            return $":{logValue.ToString()}";
        }
    }

    public enum LogValue
    {
        SourceContext,//CategoryName
        LogLevel,
        Date,
        Exception,
        State,
        EventId
    }
}
